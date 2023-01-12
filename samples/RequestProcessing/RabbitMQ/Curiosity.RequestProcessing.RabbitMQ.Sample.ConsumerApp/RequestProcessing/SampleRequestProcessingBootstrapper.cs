using Curiosity.RequestProcessing.RabbitMQ.Sample.Common;
using Curiosity.RequestProcessing;
using Curiosity.RequestProcessing.RabbitMQ;
using Curiosity.RequestProcessing.Workers;
using Microsoft.Extensions.Logging;

namespace Curiosity.RequestProcessing.RabbitMQ.Sample.ConsumerApp.RequestProcessing;

/// <summary>
/// Bootstrapper that combines all classes together and runs request processing.
/// </summary>
public class SampleRequestProcessingBootstrapper : RabbitMQRequestProcessorBootstrapperBase<
    SampleRequest,
    WorkerBasicExtraParams,
    SampleRequestProcessingWorker,
    SampleRequestDispatcher,
    SampleRequestProcessingInfo,
    SampleRequestProcessorNodeOptions>
{
    private readonly SampleRequestProcessingMetricsCollector _metricsCollector;

    /// <inheritdoc cref="RabbitMQRequestProcessorBootstrapperBase{TRequest,TWorkerParams,TWorker,TDispatcher,TProcessingRequestInfo,TOptions}"/>
    public SampleRequestProcessingBootstrapper(SampleRequestProcessorNodeOptions nodeOptions, ILoggerFactory loggerFactory, IServiceProvider serviceProvider, SampleRequestProcessorNodeOptions options, SampleRequestProcessingMetricsCollector metricsCollector) : base(nodeOptions, loggerFactory,
        serviceProvider, options)
    {
        _metricsCollector = metricsCollector;
    }

    /// <inheritdoc />
    protected override SampleRequestDispatcher CreateDispatcher(IReadOnlyList<IEventSource> monitoredDatabases)
    {
        // here we must configure and create request dispatcher

        var logger = LoggerFactory.CreateLogger<SampleRequestDispatcher>();

        return new SampleRequestDispatcher(
            NodeOptions,
            EventWaitHandle,
            CreateWorkers(NodeOptions),
            logger,
            RabbitMQReceivedEvents,
            _metricsCollector);
    }

    /// <inheritdoc />
    protected override WorkerBasicExtraParams CreateWorkerParams(string workerName, ILogger logger)
    {
        // here we can create and pass some extra params for our worker

        return new WorkerBasicExtraParams(logger);
    }
}
