using System;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.Configuration;
using Curiosity.RequestProcessing.Workers;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Curiosity.RequestProcessing.Postgres
{
    /// <summary>
    /// Базовый класс для бутстрапера обработчика запросов, который берет запросы от Postgres.
    /// </summary>
    public abstract class PostgresRequestProcessorBootstrapperBase<
        TRequest,
        TWorkerParams,
        TWorker,
        TDispatcher,
        TProcessingRequestInfo,
        TOptions> : RequestProcessorBootstrapperBase<TRequest, TWorkerParams, TWorker, TDispatcher, TProcessingRequestInfo>
        where TRequest : IRequest
        where TWorker : WorkerBase<TRequest, TWorkerParams, TProcessingRequestInfo>
        where TWorkerParams : IWorkerExtraParams
        where TDispatcher : RequestDispatcherBase<TRequest, TWorker, TWorkerParams, TProcessingRequestInfo>, IHostedService
        where TProcessingRequestInfo : class, IProcessingRequestInfo
        where TOptions : RequestProcessorNodeOptions, IPostgresRequestProcessorNodeOptions
    {
        /// <summary>
        /// Параметры получателя событий.
        /// </summary>
        protected readonly PostgresEventReceiverOptions EventReceiverOptions;

        /// <summary>
        /// <inheritdoc cref="PostgresRequestProcessorBootstrapperBase{TRequest,TWorkerParams,TWorker,TDispatcher,TProcessingRequestInfo,TOptions}"/>
        /// </summary>
        protected PostgresRequestProcessorBootstrapperBase(
            TOptions nodeOptions,
            ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider) : base(nodeOptions, loggerFactory, serviceProvider)
        {
            nodeOptions.PostgresEventReceiver.AssertValid();

            EventReceiverOptions = nodeOptions.PostgresEventReceiver;
        }

        /// <inheritdoc />
        protected sealed override async Task StartEventReceiverAsync(IEventSource eventSource, CancellationToken cancellationToken = default)
        {
            if (eventSource == null) throw new ArgumentNullException(nameof(eventSource));
            if (!(eventSource is MonitoredDatabase monitoredDatabase)) throw new InvalidOperationException($"Поддерживается только {typeof(MonitoredDatabase)}");

            if (EventReceivers.TryGetValue(monitoredDatabase, out _))
                throw new InvalidOperationException($"БД {monitoredDatabase} уже добавлена в список и прослушивается.");

            var dbEventListenerLogger = LoggerFactory.CreateLogger<DbEventReceiver>();

            var dbEventListener = new DbEventReceiver(
                EventReceiverOptions,
                monitoredDatabase,
                dbEventListenerLogger);

            // подпишемся на события в этой БД и запустим слушателя
            dbEventListener.OnEventReceived += HandleEventReceived;

            await dbEventListener.StartAsync(cancellationToken);

            // добавим слушателя в список, чтобы потом корректно завершить работу
            EventReceivers[monitoredDatabase] = dbEventListener;
        }
    }
}
