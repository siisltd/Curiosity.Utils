using System;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.AppInitializer;
using Curiosity.Tools.Performance;
using Microsoft.Extensions.Logging;

namespace Curiosity.Hosting.Performance
{
    /// <summary>
    /// Service for initializing performance monitoring services
    /// </summary>
    public class PerformanceMonitoringInitializer : IAppInitializer
    {
        private const string PerformanceLogName = "performance";
        private const string StuckCodeLogName = "stuckcode";

        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<PerformanceMonitoringInitializer> _logger;

        public PerformanceMonitoringInitializer(
            ILoggerFactory loggerFactory,
            ILogger<PerformanceMonitoringInitializer> logger)
        {
            _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public Task InitializeAsync(CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Initialization of performance monitoring service started...");
            
            _logger.LogInformation($"Initialization of {nameof(PerformanceManager)} started (using log with name {PerformanceLogName})...");
            
            var performanceLog = _loggerFactory.CreateLogger(PerformanceLogName);
            PerformanceManager.Initialize(performanceLog);
            
            _logger.LogInformation($"Initialization of {nameof(StuckCodeManager)} completed.");
            
            _logger.LogInformation($"Initialization of {nameof(StuckCodeManager)} started (using log with name {StuckCodeLogName})...");
            var stuckCodeLog = _loggerFactory.CreateLogger(StuckCodeLogName);
            StuckCodeManager.Initialize(stuckCodeLog);
            _logger.LogInformation($"Initialization of {nameof(StuckCodeManager)} completed.");
            
            _logger.LogInformation("Initialization of performance monitoring service completed.");
            
            return Task.CompletedTask;
        }
    }
}