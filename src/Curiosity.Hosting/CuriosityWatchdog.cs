using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace Curiosity.Hosting
{
    /// <summary>
    /// Базовый класс для сервисов, которые выполняют какую-то функцию по таймеру.
    /// </summary>
    public abstract class CuriosityWatchdog : BackgroundService
    {
        protected readonly ILogger _logger;
        protected readonly TimeSpan _timer;
        protected readonly TimeSpan _attemptTimer;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="logger">Логгер</param>
        /// <param name="timer">Интервал между выполнениями задачи <see cref="ProcessAsync"/></param>
        /// <param name="attemptTimer">Интервал перезапуска при возникновении ошибки</param>
        public CuriosityWatchdog(ILogger logger, TimeSpan timer, TimeSpan attemptTimer)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _timer = timer;
            _attemptTimer = attemptTimer;
        }

        /// <summary>
        /// Выполняется циклично с установленым интревалом <see cref="_timer"/>.
        /// </summary>
        protected abstract Task ProcessAsync();

        /// <summary>
        /// Устанавливает интервал <see cref="_attemptTimer"/> при возникновении ошибки.
        /// </summary>
        protected virtual async Task OnAttemptExceptionAsync(Exception e, CancellationToken stoppingToken)
        {
            _logger.LogError(e, $"Critical error in {GetType().Name}");
            // чтоб не дубасил по 500 исключений в секунду, но и не умирал на вечно
            _logger.LogInformation($"{GetType().Name} is waiting for 10 minutes and then is going to try again.");
            await Task.Delay(_attemptTimer, stoppingToken);
            _logger.LogInformation($"{GetType().Name} try to execute again.");
        }

        /// <summary>
        /// Безопасно ловит исключение и перезапускается установленный интервал <see cref="_attemptTimer"/>.
        /// </summary>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessAsync();
                    await Task.Delay(_timer, stoppingToken);
                }
                catch (Exception e) when (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogWarning(e, $"Stopping {GetType().Name}");
                }
                catch (Exception e)
                {
                    await OnAttemptExceptionAsync(e, stoppingToken);
                }
            }
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Starting {GetType().Name}...");
            await base.StartAsync(cancellationToken);
            _logger.LogInformation($"Starting {GetType().Name} completed.");
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Stopping {GetType().Name}...");
            await base.StopAsync(cancellationToken);
            _logger.LogInformation($"Stopping {GetType().Name} completed.");
        }
    }
}
