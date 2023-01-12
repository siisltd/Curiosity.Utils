using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.RequestProcessing.RabbitMQ.Options;
using Curiosity.RequestProcessing.Workers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Curiosity.RequestProcessing.RabbitMQ;

/// <summary>
/// Base class for bootstrapping processing request via RabbitMQ.
/// </summary>
public abstract class RabbitMQRequestProcessorBootstrapperBase<
    TRequest,
    TWorkerParams,
    TWorker,
    TDispatcher,
    TProcessingRequestInfo,
    TOptions> : RequestProcessorBootstrapperBase<RabbitMQRequestWrapper<TRequest>, TWorkerParams, TWorker, TDispatcher, TProcessingRequestInfo, TOptions>
    where TRequest : IRequest
    where TWorker : WorkerBase<RabbitMQRequestWrapper<TRequest>, TWorkerParams, TProcessingRequestInfo, TOptions>
    where TWorkerParams : IWorkerExtraParams
    where TDispatcher : RequestDispatcherBase<RabbitMQRequestWrapper<TRequest>, TWorker, TWorkerParams, TProcessingRequestInfo, TOptions>, IHostedService
    where TProcessingRequestInfo : class, IProcessingRequestInfo
    where TOptions : RequestProcessorNodeOptions, IRabbitMQRequestProcessorNodeOptions
{
    private readonly TOptions _options;

    /// <summary>
    /// Received events from RabbitMQ.
    /// </summary>
    protected ConcurrentQueue<RabbitMQEvent> RabbitMQReceivedEvents { get; }

    /// <inheritdoc cref="RabbitMQRequestProcessorBootstrapperBase{TRequest,TWorkerParams,TWorker,TDispatcher,TProcessingRequestInfo,TOptions}"/>
    protected RabbitMQRequestProcessorBootstrapperBase(
        TOptions nodeOptions,
        ILoggerFactory loggerFactory,
        IServiceProvider serviceProvider,
        TOptions options) : base(nodeOptions, loggerFactory, serviceProvider)
    {
        _options = options;
        RabbitMQReceivedEvents = new ConcurrentQueue<RabbitMQEvent>();
    }

    /// <inheritdoc />
    protected override IReadOnlyList<IEventSource> GetEventSources()
    {
        return new[] { new RabbitMQEventSource(_options.RabbitMQEventReceiver) };
    }

    /// <inheritdoc />
    protected override async Task StartEventReceiverAsync(
        IEventSource eventSource,
        CancellationToken cancellationToken = default)
    {
        if (eventSource == null) throw new ArgumentNullException(nameof(eventSource));
        if (!(eventSource is RabbitMQEventSource rabbitMQEventSource)) throw new InvalidOperationException($"Only {typeof(RabbitMQEventSource)} is supported");

        // create and configure receiver

        var logger = LoggerFactory.CreateLogger<RabbitMQEventReceiver>();

        var receiver = new RabbitMQEventReceiver(
            rabbitMQEventSource.RabbitMQOptions,
            logger,
            NodeOptions.WorkersCount);

        receiver.OnEventReceived += HandleEventReceived!;

        // start receiving
        await receiver.StartAsync(cancellationToken);

        // add to list of receivers
        EventReceivers[rabbitMQEventSource] = receiver;
    }

    /// <inheritdoc />
    protected override void HandleEventReceived(object sender, IRequestProcessingEvent e)
    {
        // add event to internal queue
        if (!(e is RabbitMQEvent rabbitMQEvent)) throw new InvalidOperationException($"Only {typeof(RabbitMQEvent)} is supported");
        RabbitMQReceivedEvents.Enqueue(rabbitMQEvent);

        Logger.LogDebug("Internal received events queue size: {QueueSize}", RabbitMQReceivedEvents.Count);

        // call base method to notify dispatcher
        base.HandleEventReceived(sender, e);
    }
}
