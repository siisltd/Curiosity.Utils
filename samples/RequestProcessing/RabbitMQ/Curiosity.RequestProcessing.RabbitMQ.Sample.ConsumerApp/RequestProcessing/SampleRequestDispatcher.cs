using System.Collections.Concurrent;
using System.Globalization;
using System.Text;
using Curiosity.RequestProcessing.RabbitMQ.Sample.Common;
using Curiosity.RequestProcessing.RabbitMQ;
using Curiosity.RequestProcessing.Workers;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Curiosity.RequestProcessing.RabbitMQ.Sample.ConsumerApp.RequestProcessing;

/// <summary>
/// Dispatcher that receives request from RabbitMQ, dispatches it to worker and confirm/reject request after processing/errors.
/// </summary>
public class SampleRequestDispatcher : RabbitMQRequestDispatcherBase<
    SampleRequest,
    SampleRequestProcessingWorker,
    WorkerBasicExtraParams,
    SampleRequestProcessingInfo,
    SampleRequestProcessorNodeOptions>
{
    private readonly SampleRequestProcessingMetricsCollector _metricsCollector;

    /// <inheritdoc cref="SampleRequestDispatcher"/>
    public SampleRequestDispatcher(
        SampleRequestProcessorNodeOptions nodeOptions,
        EventWaitHandle manualResetEvent,
        IReadOnlyList<SampleRequestProcessingWorker> workers,
        ILogger logger,
        ConcurrentQueue<RabbitMQEvent> receivedEvents,
        SampleRequestProcessingMetricsCollector metricsCollector) : base(nodeOptions, manualResetEvent, workers, logger, receivedEvents)
    {
        _metricsCollector = metricsCollector ?? throw new ArgumentNullException(nameof(metricsCollector));
    }

    /// <inheritdoc />
    protected override Task<IReadOnlyList<RabbitMQRequestWrapper<SampleRequest>>?> GetRequestsAsync(int maxRequestsCount, CancellationToken cancellationToken = default)
    {
        // here we must fetch request from internal queue, parse it and prepare for processing

        string? jsonData = null;
        string? correlationId = null;
        var result = new List<RabbitMQRequestWrapper<SampleRequest>>(maxRequestsCount);

         // just read requests from the inner queue
         while (result.Count < maxRequestsCount && ReceivedEvents.TryDequeue(out var rabbitMQEvent))
         {
             try
             {
                 // parse it
                 jsonData = Encoding.UTF8.GetString(rabbitMQEvent.Payload);
                 correlationId = rabbitMQEvent.ReceivedData.BasicProperties.CorrelationId;
                 var checkRequest = JsonConvert.DeserializeObject<SampleRequest>(jsonData) ?? throw new ArgumentNullException(nameof(jsonData));

                 // and prepare for processing
                 var request = new RabbitMQRequestWrapper<SampleRequest>(
                     checkRequest.Id,
                     CultureInfo.CurrentCulture,
                     checkRequest,
                     rabbitMQEvent);

                 result.Add(request);
             }
             catch (Exception e)
             {
                 Logger.LogError(
                     e,
                     "Failed to fetch requests from internal queue (CorrelationId=\"{CorrelationId}\", DeliveryTag={DeliveryTag}). Request will be accepted to clean up queue. Received json data: \"{JsonData}\"",
                     correlationId,
                     rabbitMQEvent?.ReceivedData.DeliveryTag,
                     jsonData ?? "<none>");

                 // confirm to clean up queue
                 if (rabbitMQEvent != null)
                 {
                     rabbitMQEvent.ConfirmProcessing();
                 }
             }
         }

         return Task.FromResult(result as IReadOnlyList<RabbitMQRequestWrapper<SampleRequest>?>);
    }

    /// <inheritdoc />
    protected override Task HandleRequestProcessingStartedAsync(RabbitMQRequestWrapper<SampleRequest> request, CancellationToken cancellationToken = default)
    {
        // here we can make something before processing of request started
        // for example, update metrics

        _metricsCollector.MarkProcessingStarted();

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override Task HandleRequestProcessingCompletionAsync(RabbitMQRequestWrapper<SampleRequest> request, bool isSuccessful, CancellationToken cancellationToken = default)
    {               
        // here we can make something on completion of request processing 
        // for example, update metrics
        _metricsCollector.MarkProcessingCompleted(isSuccessful);

        // execute basic logic of completion - complete or reject request
        return base.HandleRequestProcessingCompletionAsync(request, isSuccessful, cancellationToken);
    }
}
