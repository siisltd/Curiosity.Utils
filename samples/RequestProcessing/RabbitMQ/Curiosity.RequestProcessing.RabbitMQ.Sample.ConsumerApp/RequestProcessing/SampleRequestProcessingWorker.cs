using Curiosity.RequestProcessing.RabbitMQ.Sample.Common;
using Curiosity.RequestProcessing.RabbitMQ;
using Curiosity.RequestProcessing.Workers;
using Microsoft.Extensions.Logging;

namespace Curiosity.RequestProcessing.RabbitMQ.Sample.ConsumerApp.RequestProcessing;

/// <summary>
/// Worker that process sample request.
/// </summary>
public class SampleRequestProcessingWorker : WorkerBase<
    RabbitMQRequestWrapper<SampleRequest>,
    WorkerBasicExtraParams,
    SampleRequestProcessingInfo,
    SampleRequestProcessorNodeOptions>
{
    private readonly Random _random;

    /// <inheritdoc cref="SampleRequestProcessingWorker"/>
    public SampleRequestProcessingWorker(SampleRequestProcessorNodeOptions nodeOptions) : base(nodeOptions)
    {
        _random = new Random(Environment.TickCount);
    }

    /// <inheritdoc />
    protected override async Task ProcessRequestAsync(RabbitMQRequestWrapper<SampleRequest> requestWrapper, CancellationToken cancellationToken)
    {
        Logger.LogInformation("Started processing request with RequestId={RequestId}", requestWrapper.Id);

         // sometimes throw exception to show who request can't be rejected and re-queued
         if (_random.Next(0, 100) > 98)
         {
             throw new InvalidOperationException("Oh, it's exception time!");
         }

         // emulate some work
         var delayMs = _random.Next(1, 100);
         await Task.Delay(delayMs, cancellationToken);

         Logger.LogInformation("Completed processing request with RequestId={RequestId}", requestWrapper.Id);
    }

    /// <inheritdoc />
    protected override Task HandleExceptionAsync(RabbitMQRequestWrapper<SampleRequest> requestWrapper, Exception ex)
    {
        Logger.LogError(ex, "Error while processing request with RequestId={RequestId}", requestWrapper.Id);
        
        return Task.CompletedTask;
    }

    /// <inheritdoc />
    protected override SampleRequestProcessingInfo GetRequestInfo(RabbitMQRequestWrapper<SampleRequest> requestWrapper)
    {
        return new SampleRequestProcessingInfo(DateTime.UtcNow);
    }
}
