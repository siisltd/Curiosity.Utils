using System;
using Curiosity.Configuration;
using Curiosity.RequestProcessing.RabbitMQ.Options;
using Curiosity.RequestProcessing.Workers;
using Microsoft.Extensions.DependencyInjection;

namespace Curiosity.RequestProcessing.RabbitMQ;

/// <summary>
/// Extension methods for <see cref="IServiceCollection"/> to register RabbitMQ request processing services.
/// </summary>
public static class IocExtensions
{
    /// <summary>
    /// Adds services for processing requests from RabbitMQ.
    /// </summary>
    public static void AddRabbitMQRequestProcessor<
        TRequest,
        TWorker,
        TWorkerParams,
        TProcessorBootstrapper,
        TOptions,
        TDispatcher,
        TProcessingRequestInfo>(
        this IServiceCollection services,
        TOptions processorNodeOptions)
        where TRequest : IRequest
        where TWorkerParams : class, IWorkerExtraParams
        where TDispatcher : RabbitMQRequestDispatcherBase<TRequest, TWorker, TWorkerParams, TProcessingRequestInfo, TOptions>
        where TWorker : WorkerBase<RabbitMQRequestWrapper<TRequest>, TWorkerParams, TProcessingRequestInfo, TOptions>
        where TOptions : RequestProcessorNodeOptions, IRabbitMQRequestProcessorNodeOptions
        where TProcessorBootstrapper : RabbitMQRequestProcessorBootstrapperBase<TRequest, TWorkerParams, TWorker, TDispatcher, TProcessingRequestInfo, TOptions>
        where TProcessingRequestInfo : class, IProcessingRequestInfo
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (processorNodeOptions == null) throw new ArgumentNullException(nameof(processorNodeOptions));

        processorNodeOptions.AssertValid();

        services.AddRequestProcessor<
            RabbitMQRequestWrapper<TRequest>,
            TWorker,
            TWorkerParams,
            TProcessorBootstrapper,
            TOptions,
            TDispatcher,
            TProcessingRequestInfo>(processorNodeOptions);
    }
}
