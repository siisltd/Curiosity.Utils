using System;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Curiosity.DateTime.DbSync
{
    /// <summary>
    /// Service of periodic synchronization date and time with database.
    /// </summary>
    public class DbDateTimeSynchronizationWatchdog<T> : BackgroundService where T: DbSyncDateTimeService
    {
        private readonly DbDateTimeOptions _dbDateTimeOptions;
        private readonly T _dateTimeService;
        private readonly ILogger _logger;

        public DbDateTimeSynchronizationWatchdog(
            DbDateTimeOptions dbDateTimeOptions,
            T dateTimeService,
            ILogger<DbDateTimeSynchronizationWatchdog<T>> logger)
        {
            _dbDateTimeOptions = dbDateTimeOptions ?? throw new ArgumentNullException(nameof(dbDateTimeOptions));
            _dbDateTimeOptions.AssertValid();
            
            _dateTimeService = dateTimeService ?? throw new ArgumentNullException(nameof(dateTimeService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Starting date time with DB synchronization watchdog with check period of {_dbDateTimeOptions.SyncPeriodMin} minutes...");
            
            await base.StartAsync(cancellationToken);
            
            _logger.LogInformation("Date time with DB synchronization watchdog started.");
        }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(TimeSpan.FromMinutes(_dbDateTimeOptions.SyncPeriodMin), stoppingToken);
                    _logger.LogTrace("Synchronization date and time with DB started...");
                    await _dateTimeService.InitAsync(stoppingToken);   
                    _logger.LogTrace("Synchronization date and time with DB completed.");
                }
                catch (Exception e) when (stoppingToken.IsCancellationRequested)
                {
                    _logger.LogInformation(e, "Sync data and time with DB was cancelled.");
                }
                catch (Exception e)
                {
                    _logger.LogError(e, $"Error while sync data and time with DB. Reason: {e.Message}.");
                }
            }
        }

        /// <inheritdoc />
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping date time with DB synchronization watchdog...");
            
            await base.StopAsync(cancellationToken);
            
            _logger.LogInformation("Date time with DB synchronization watchdog stopped.");
        }
    }
}
