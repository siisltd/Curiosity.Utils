using System;
using Curiosity.Configuration;
using Curiosity.RequestProcessing.Workers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Curiosity.RequestProcessing.Postgres
{
    public static class RequestProcessorIocExtensions
    {
        /// <summary>
        /// Добавляет основные сервисы для обработки запросов.
        /// </summary>
        public static void AddPostgresRequestProcessor<
            TRequest,
            TRequestEntity,
            TWorker,
            TWorkerParams,
            TProcessorBootstrapper,
            TOptions,
            TDispatcher,
            TProcessingRequestInfo>(
            this IServiceCollection services,
            TOptions processorNodeOptions)
            where TRequest : IRequest
            where TRequestEntity : class
            where TWorkerParams : class, IWorkerExtraParams
            where TDispatcher : RequestDispatcherBase<TRequest, TRequestEntity, TWorker, TWorkerParams, TProcessingRequestInfo>
            where TWorker : WorkerBase<TRequest, TWorkerParams, TProcessingRequestInfo>
            where TOptions : RequestProcessorNodeOptions, IPostgresRequestProcessorNodeOptions
            where TProcessorBootstrapper : PostgresRequestProcessorBootstrapperBase<TRequest, TRequestEntity, TWorkerParams, TWorker, TDispatcher, TProcessingRequestInfo, TOptions>
            where TProcessingRequestInfo : class, IProcessingRequestInfo
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (processorNodeOptions == null) throw new ArgumentNullException(nameof(processorNodeOptions));
            processorNodeOptions.AssertValid();

            services.AddRequestProcessor<TRequest, TRequestEntity, TWorker, TWorkerParams, TProcessorBootstrapper, TOptions, TDispatcher, TProcessingRequestInfo>(processorNodeOptions);
        }
    }
}
