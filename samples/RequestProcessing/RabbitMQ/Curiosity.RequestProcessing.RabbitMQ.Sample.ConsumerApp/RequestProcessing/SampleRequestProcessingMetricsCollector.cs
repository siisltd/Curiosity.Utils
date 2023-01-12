using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Curiosity.RequestProcessing.RabbitMQ.Sample.ConsumerApp.RequestProcessing;

/// <summary>
/// Metrics collector for sample request processing;
/// </summary>
public class SampleRequestProcessingMetricsCollector : BackgroundService
{
    private static readonly TimeSpan MetricFlushPeriod = TimeSpan.FromSeconds(1);

    private volatile int _activeWorkersCount;
    private volatile int _processedRequestFromLastLog;
    private volatile int _totalSuccessfulRequestsCount;
    private volatile int _totalFailedRequestsCount;

    private readonly ILogger _logger;

    /// <inheritdoc cref="SampleRequestProcessingMetricsCollector"/>
    public SampleRequestProcessingMetricsCollector(ILogger<SampleRequestProcessingMetricsCollector> logger)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Marks that some worker started processing request.
    /// </summary>
    public void MarkProcessingStarted()
    {
        Interlocked.Increment(ref _activeWorkersCount);
    }

    /// <summary>
    /// Marks that some worker completed processing request.
    /// </summary>
    public void MarkProcessingCompleted(bool isSuccessful)
    {
        Interlocked.Decrement(ref _activeWorkersCount);
        Interlocked.Increment(ref _processedRequestFromLastLog);

        if (isSuccessful)
        {
            Interlocked.Increment(ref _totalSuccessfulRequestsCount);
        }
        else
        {
            Interlocked.Increment(ref _totalFailedRequestsCount);
        }
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                // collect metrics
                var currentActiveWorkers = _activeWorkersCount;
                var processedRequestsFromLastLog = _processedRequestFromLastLog;
                Interlocked.Exchange(ref _processedRequestFromLastLog, 0);
                var totalSuccessfulRequests = _totalSuccessfulRequestsCount;
                var totalFailedRequests = _totalFailedRequestsCount;

                // print metrics
                _logger.LogInformation(
                    "Current state: ActiveWorkers={ActiveWorkers}, RPS={RPS}, SuccessfulRequests={SuccessfulRequests}, FailedRequests={FailedRequests}",
                    currentActiveWorkers,
                    processedRequestsFromLastLog,
                    totalSuccessfulRequests,
                    totalFailedRequests);

                // sleep
                await Task.Delay(MetricFlushPeriod, stoppingToken);

                // repeat
            }
            catch (Exception) when (stoppingToken.IsCancellationRequested)
            {
                // no action
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error while collecting metrics");
            }
        }
    }
}
