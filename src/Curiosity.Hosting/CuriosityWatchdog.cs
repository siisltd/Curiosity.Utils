using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Curiosity.Hosting
{
    /// <summary>
    /// A base class for services that perform some function on a timer.
    /// </summary>
    public abstract class CuriosityWatchdog : BackgroundService
    {
        protected ILogger Logger { get; }

        /// <summary>
        /// Interval between task executions
        /// </summary>
        protected TimeSpan WatchdogPeriod { get; }

        /// <summary>
        /// Restart interval when an error occurs, default is 10 minutes
        /// </summary>
        protected TimeSpan ExceptionDelay { get; } = TimeSpan.FromMinutes(10);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger">Logger</param>
        /// <param name="watchdogPeriod">Interval between task executions <see cref="ProcessAsync"/></param>
        /// <param name="exceptionDelay">Restart interval when an error occurs, default is 10 minutes</param>
        public CuriosityWatchdog(ILogger logger, TimeSpan watchdogPeriod, TimeSpan? exceptionDelay = null)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            WatchdogPeriod = watchdogPeriod;

            if (exceptionDelay.HasValue)
                ExceptionDelay = exceptionDelay.Value;
        }

        /// <summary>
        /// It is performed cyclically with a set interval <see cref="WatchdogPeriod"/>.
        /// </summary>
        protected abstract Task ProcessAsync();

        /// <summary>
        /// Handles the exception with the set interval <see cref="ExceptionDelay"/>.
        /// </summary>
        protected virtual async Task HandleExceptionAsync(Exception e, CancellationToken stoppingToken = default)
        {
            Logger.LogError(e, $"Critical error in {GetType().Name}");
            // So that it doesn't throw 500 exceptions per second, but also doesn't die forever
            Logger.LogInformation($"{GetType().Name} is waiting for 10 minutes and then is going to try again.");
            await Task.Delay(ExceptionDelay, stoppingToken);
            Logger.LogInformation($"{GetType().Name} try to execute again.");
        }

        /// <summary>
        /// Safely catches the exception and restarts the set interval <see cref="ExceptionDelay"/>.
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken = default)
        {
            await Task.Yield();
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessAsync();
                    await Task.Delay(WatchdogPeriod, stoppingToken);
                }
                catch (Exception e) when (stoppingToken.IsCancellationRequested)
                {
                    Logger.LogWarning(e, $"Stopping {GetType().Name}");
                }
                catch (Exception e)
                {
                    await HandleExceptionAsync(e, stoppingToken);
                }
            }
        }

        public override async Task StartAsync(CancellationToken cancellationToken = default)
        {
            Logger.LogInformation($"Starting {GetType().Name}...");
            await base.StartAsync(cancellationToken);
            Logger.LogInformation($"Starting {GetType().Name} completed.");
        }

        public override async Task StopAsync(CancellationToken cancellationToken = default)
        {
            Logger.LogInformation($"Stopping {GetType().Name}...");
            await base.StopAsync(cancellationToken);
            Logger.LogInformation($"Stopping {GetType().Name} completed.");
        }
    }
}
