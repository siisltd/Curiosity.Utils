using System;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Curiosity.Hosting.ThreadPool
{
    /// <summary>
    /// Service for monitoring and auto tuning of <see cref="ThreadPool"/>.
    /// </summary>
    public class ThreadPoolMonitoringService : BackgroundService
    {
        private readonly ThreadPoolOptions _options;
        private readonly ILogger _logger;
        
        /// <inheritdoc />
        public ThreadPoolMonitoringService(
            ThreadPoolOptions options,
            ILogger logger)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            options.AssertValid();
            
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting thread pool monitoring service...");
            await base.StartAsync(cancellationToken);
            _logger.LogInformation("Thread pool monitoring service started.");
        }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_options.AutoTune && _options.LogThreadPoolStatePeriodSec <= 0)
            {
                _logger.LogInformation("Thread pool monitoring service exiting because of nothing to do");
                return;
            }

            _logger.LogInformation($"Thread pool auto tuning {(_options.AutoTune ? "enabled" : "disabled")}");

            var ticks = Environment.TickCount;
            var autoTuneIncreaseTicks = ticks;
            var autoTuneDecreaseTicks = ticks;
            var logThreadPoolStateTicks = ticks;
            
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    ticks = Environment.TickCount;

                    // wait for tune or log moment
                    var delay = (int?)null;
                    if (_options.AutoTune)
                    {
                        delay = Math.Min(autoTuneIncreaseTicks - ticks, autoTuneDecreaseTicks - ticks);
                    }
                    if (_options.LogThreadPoolStatePeriodSec > 0)
                    {
                        if (delay.HasValue)
                        {
                            delay = Math.Min(logThreadPoolStateTicks - ticks, delay.Value);
                        }
                        else
                        {
                            delay = logThreadPoolStateTicks - ticks;
                        }
                    }
                    if (!delay.HasValue)
                        throw new InvalidOperationException("Something wrong with logic here");
                    
                    await Task.Delay(Math.Max(delay.Value, 0), stoppingToken);

                    // select an action: increase, decrease or log
                    ticks = Environment.TickCount;
                    var doIncreaseCheck = _options.AutoTune && autoTuneIncreaseTicks <= ticks;
                    var doDecreaseCheck = _options.AutoTune && autoTuneDecreaseTicks <= ticks;
                    var doLogState = logThreadPoolStateTicks <= ticks;

                    if (doIncreaseCheck || doDecreaseCheck)
                    {
                        var doWorkerThreadsDecreaseCheck = doDecreaseCheck;
                        var doCompletionPortThreadsDecreaseCheck = doDecreaseCheck;
                        
                        System.Threading.ThreadPool.GetMinThreads(
                            out var minWorkerThreads,
                            out var minCompletionPortThreads);
                        System.Threading.ThreadPool.GetMaxThreads(
                            out var maxWorkerThreads,
                            out var maxCompletionPortThreads);
                        System.Threading.ThreadPool.GetAvailableThreads(
                            out var availableWorkerThreads,
                            out var availableCompletionPortThreads);

                        var usedWorkerThreads = maxWorkerThreads - availableWorkerThreads;
                        var unusedWorkerThreads = Math.Max(minWorkerThreads - usedWorkerThreads, 0);

                        var usedCompletionPortThreads = maxCompletionPortThreads - availableCompletionPortThreads;
                        var unusedCompletionPortThreads = Math.Max(minCompletionPortThreads - usedCompletionPortThreads, 0);

                        int? newMinWorkerThreads = null;
                        int? newMinCompletionPortThreads = null;

                        // сначала проверяем на недостаток свободных потоков
                        if (doIncreaseCheck)
                        {
                            if (unusedWorkerThreads < _options.AutoTuneExpectedUnusedWorkerThreads)
                            {
                                newMinWorkerThreads = usedWorkerThreads + unusedWorkerThreads + _options.AutoTuneIncreaseUnusedWorkerThreadsPerStep;

                                // при этом уже не надо нам проверять на избыток потоков
                                doWorkerThreadsDecreaseCheck = false;
                                autoTuneDecreaseTicks = ticks + (_options.AutoTuneDecreasePeriodSec * 1000);
                            }

                            if (unusedCompletionPortThreads < _options.AutoTuneExpectedUnusedCompletionPortThreads)
                            {
                                newMinCompletionPortThreads = usedCompletionPortThreads + unusedCompletionPortThreads + _options.AutoTuneIncreaseUnusedCompletionPortThreadsPerStep;

                                // при этом уже не надо нам проверять на избыток потоков
                                doCompletionPortThreadsDecreaseCheck = false;
                                autoTuneDecreaseTicks = ticks + (_options.AutoTuneDecreasePeriodSec * 1000);
                            }
                            
                            autoTuneIncreaseTicks = ticks + (_options.AutoTuneIncreasePeriodSec * 1000);
                        }

                        // если надо, то проверяем избыток неиспользуемых потоков
                        if (doDecreaseCheck)
                        {
                            if (doWorkerThreadsDecreaseCheck && unusedWorkerThreads > _options.AutoTuneExpectedUnusedWorkerThreads)
                            {
                                newMinWorkerThreads = usedWorkerThreads + unusedWorkerThreads - _options.AutoTuneDecreaseUnusedWorkerThreadsPerStep;
                            }
                            if (doCompletionPortThreadsDecreaseCheck && unusedCompletionPortThreads > _options.AutoTuneExpectedUnusedCompletionPortThreads)
                            {
                                newMinCompletionPortThreads = usedCompletionPortThreads + unusedCompletionPortThreads - _options.AutoTuneDecreaseUnusedCompletionPortThreadsPerStep;
                            }

                            autoTuneDecreaseTicks = ticks + (_options.AutoTuneDecreasePeriodSec * 1000);
                        }

                        // если таки решили менять - меняем
                        if (newMinWorkerThreads.HasValue || newMinCompletionPortThreads.HasValue)
                        {
                            // нельзя чтобы расчетное значение были меньше заданного в конфиге минимума 
                            newMinWorkerThreads = Math.Max(newMinWorkerThreads ?? minWorkerThreads, Math.Max(_options.MinWorkerThreads, 0));
                            newMinCompletionPortThreads = Math.Max(newMinCompletionPortThreads ?? minCompletionPortThreads, Math.Max(_options.MinCompletionPortThreads, 0));

                            if (newMinWorkerThreads.Value != minWorkerThreads || newMinCompletionPortThreads.Value != minCompletionPortThreads)
                            {
                                var change1 = newMinWorkerThreads.Value - minWorkerThreads;
                                var changeStr1 = (change1 == 0) ? String.Empty : "[" + (change1 < 0 ? change1.ToString() : "+" + change1.ToString()) + "]";

                                var change2 = newMinCompletionPortThreads.Value - minCompletionPortThreads;
                                var changeStr2 = (change2 == 0) ? String.Empty : "[" + (change2 < 0 ? change2.ToString() : "+" + change2.ToString()) + "]";
                                
                                _logger.LogInformation($"Thread pool: set minimum worker threads = {newMinWorkerThreads.Value}{changeStr1} (used = {usedWorkerThreads}), I/O threads = {newMinCompletionPortThreads.Value}{changeStr2} (used = {usedCompletionPortThreads})");
                                System.Threading.ThreadPool.SetMinThreads(newMinWorkerThreads.Value, minCompletionPortThreads);
                            }
                        }
                    }

                    // нужен дамп состояния?
                    if (doLogState)
                    {
                        LogThreadPoolState();
                        logThreadPoolStateTicks = ticks + (_options.LogThreadPoolStatePeriodSec * 1000);
                    }
                }
                catch (Exception e) when (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation(e, "Monitoring of thread pool was cancelled.");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error on monitoring of thread pool. Reason: {e.Message}.");
                }
            }
        }

        // выводим в журнал информацию о текущем состоянии пула потоков
        private void LogThreadPoolState()
        {
            System.Threading.ThreadPool.GetMinThreads(
                out var minWorkerThreads,
                out var minCompletionPortThreads);
            System.Threading.ThreadPool.GetMaxThreads(
                out var maxWorkerThreads,
                out var maxCompletionPortThreads);
            System.Threading.ThreadPool.GetAvailableThreads(
                out var availableWorkerThreads,
                out var availableCompletionPortThreads);

            var usedWorkerThreads = maxWorkerThreads - availableWorkerThreads;
            var usedCompletionPortThreads = maxCompletionPortThreads - availableCompletionPortThreads;
            
            _logger.LogInformation($"Thread pool: worker threads = {usedWorkerThreads} ({minWorkerThreads}...{maxWorkerThreads}), I/O threads = {usedCompletionPortThreads} ({minCompletionPortThreads}...{maxCompletionPortThreads})");
        }

        /// <inheritdoc />
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping thread pool monitoring service...");
            await base.StopAsync(cancellationToken);
            _logger.LogInformation("Stopping thread pool monitoring service stopped.");
        }
    }
}