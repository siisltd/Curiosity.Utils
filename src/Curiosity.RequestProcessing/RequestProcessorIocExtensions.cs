using System;
using Curiosity.Configuration;
using Curiosity.RequestProcessing.Workers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Curiosity.RequestProcessing
{
    /// <summary>
    /// Методы расширения по регистрации обработчиков запросов.
    /// </summary>
    public static class RequestProcessorIocExtensions
    {
        /// <summary>
        /// Добавляет основные сервисы для обработки запросов.
        /// </summary>
        public static void AddRequestProcessor<
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
            where TDispatcher : RequestDispatcherBase<TRequest, TWorker, TWorkerParams, TProcessingRequestInfo>
            where TWorker : WorkerBase<TRequest, TWorkerParams, TProcessingRequestInfo>
            where TOptions : RequestProcessorNodeOptions
            where TProcessorBootstrapper : RequestProcessorBootstrapperBase<TRequest, TWorkerParams, TWorker, TDispatcher, TProcessingRequestInfo>
            where TProcessingRequestInfo : class, IProcessingRequestInfo
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (processorNodeOptions == null) throw new ArgumentNullException(nameof(processorNodeOptions));
            processorNodeOptions.AssertValid();

            services.AddTransient<TWorker>();

            services.AddSingleton<TProcessorBootstrapper>();
            services.AddSingleton<IHostedService>(p => p.GetRequiredService<TProcessorBootstrapper>());

            services.AddSingleton(processorNodeOptions);
            services.AddSingleton<RequestProcessorNodeOptions>(c => c.GetRequiredService<TOptions>());
        }
    }
}
