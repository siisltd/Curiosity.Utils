using System;
using Curiosity.Configuration;
using Curiosity.RequestProcessing.Workers;
using Microsoft.Extensions.DependencyInjection;

namespace Curiosity.RequestProcessing.Postgres
{
    public static class RequestProcessorIocExtensions
    {
        /// <summary>
        /// Добавляет основные сервисы для обработки запросов.
        /// </summary>
        public static void AddPostgresRequestProcessor<
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
            where TDispatcher : RequestDispatcherBase<TRequest, TWorker, TWorkerParams, TProcessingRequestInfo, TOptions>
            where TWorker : WorkerBase<TRequest, TWorkerParams, TProcessingRequestInfo, TOptions>
            where TOptions : RequestProcessorNodeOptions, IPostgresRequestProcessorNodeOptions
            where TProcessorBootstrapper : PostgresRequestProcessorBootstrapperBase<TRequest, TWorkerParams, TWorker, TDispatcher, TProcessingRequestInfo, TOptions>
            where TProcessingRequestInfo : class, IProcessingRequestInfo
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (processorNodeOptions == null) throw new ArgumentNullException(nameof(processorNodeOptions));
            processorNodeOptions.AssertValid();

            services.AddRequestProcessor<TRequest, TWorker, TWorkerParams, TProcessorBootstrapper, TOptions, TDispatcher, TProcessingRequestInfo>(processorNodeOptions);
        }
    }
}
