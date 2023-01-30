using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.RequestProcessing.RabbitMQ.Options;
using Curiosity.RequestProcessing.Workers;
using Microsoft.Extensions.Logging;

namespace Curiosity.RequestProcessing.RabbitMQ;

/// <summary>
/// Base class for dispatchers that receives requests from RabbitMQ.
/// </summary>
/// <typeparam name="TRequest">Type of processing request.</typeparam>
/// <typeparam name="TWorker">Type of worker.</typeparam>
/// <typeparam name="TWorkerExtraParams">Type of extra params for worker.</typeparam>
/// <typeparam name="TProcessingRequestInfo">Type of information about request processing by worker.</typeparam>
/// <typeparam name="TNodeOptions">Type of processor node options</typeparam>
public abstract class RabbitMQRequestDispatcherBase<TRequest, TWorker, TWorkerExtraParams, TProcessingRequestInfo, TNodeOptions> : RequestDispatcherBase<RabbitMQRequestWrapper<TRequest>, TWorker, TWorkerExtraParams, TProcessingRequestInfo, TNodeOptions> 
    where TRequest : IRequest
    where TWorker : WorkerBase<RabbitMQRequestWrapper<TRequest>, TWorkerExtraParams, TProcessingRequestInfo, TNodeOptions>
    where TWorkerExtraParams : IWorkerExtraParams
    where TProcessingRequestInfo : class, IProcessingRequestInfo
    where TNodeOptions: RequestProcessorNodeOptions, IRabbitMQRequestProcessorNodeOptions 
{
    /// <summary>
    /// Internal queue of received events from RabbitMQ. This events should be processed by workers.
    /// After processing each event should be confirmed or rejected.
    /// </summary>
    protected ConcurrentQueue<RabbitMQEvent> ReceivedEvents { get; }

    /// <inheritdoc cref="RabbitMQRequestDispatcherBase{TRequest,TWorker,TWorkerExtraParams,TProcessingRequestInfo,TNodeOptions}"/>
    protected RabbitMQRequestDispatcherBase(
        TNodeOptions nodeOptions,
        EventWaitHandle manualResetEvent,
        IReadOnlyList<TWorker> workers,
        ILogger logger,
        ConcurrentQueue<RabbitMQEvent> receivedEvents) : base(nodeOptions, manualResetEvent, workers, logger)
    {
        ReceivedEvents = receivedEvents ?? throw new ArgumentNullException(nameof(receivedEvents));
    }

    /// <inheritdoc />
    protected override Task HandleRequestProcessingCompletionAsync(RabbitMQRequestWrapper<TRequest> request, bool isSuccessful, CancellationToken cancellationToken = default)
    {
        // confirm or reject event 
        if (isSuccessful)
        {
            request.ConfirmProcessing();
        }
        else
        {
            request.RejectProcessing();
        }

        Logger.LogDebug("Internal received events queue size: {QueueSize}", ReceivedEvents.Count);

        return Task.CompletedTask;
    }
}
