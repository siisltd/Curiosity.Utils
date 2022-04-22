using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.Configuration;
using Curiosity.RequestProcessing.Workers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Curiosity.RequestProcessing
{
    /// <summary>
    /// Бустрапер сервисов обработки запросов: подключается к БД, слушает события и управляет жизненным циклом диспетчера обработки запросов, а также создает воркеры.
    /// </summary>
    /// <typeparam name="TRequest">Тип POCO класса запроса, который будет обрабатывать <see cref="TWorker"/>.</typeparam>
    /// <typeparam name="TWorkerParams">Параметры для создания воркера, которые нельзя получить из DI (например, логер, созданный с именем воркера)</typeparam>
    /// <typeparam name="TWorker">Тип воркера, который будет обрабатывать запросы.</typeparam>
    /// <typeparam name="TDispatcher">Тип диспетчера, который будет раскидывать запросы <see cref="TRequest"/> по воркерам <see cref="TWorker"/>.</typeparam>
    /// <typeparam name="TProcessingRequestInfo">Информация о запросе, который воркер обрабатывает в данный момент.</typeparam>
    public abstract class RequestProcessorBootstrapperBase<
        TRequest,
        TWorkerParams,
        TWorker,
        TDispatcher,
        TProcessingRequestInfo> : BackgroundService
        where TRequest : IRequest
        where TWorker : WorkerBase<TRequest, TWorkerParams, TProcessingRequestInfo>
        where TWorkerParams : IWorkerExtraParams
        where TDispatcher: RequestDispatcherBase<TRequest, TWorker, TWorkerParams, TProcessingRequestInfo>, IHostedService
        where TProcessingRequestInfo : class, IProcessingRequestInfo
    {
        /// <summary>
        /// Слушатели событий в БД (можно одновременно слушать события из разных БД).
        /// </summary>
        protected readonly ConcurrentDictionary<IEventSource, IEventReceiver> EventReceivers;

        /// <summary>
        /// Сконфигурированный ServiceProvider.
        /// </summary>
        protected IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Общие параметры обработчика запросов.
        /// </summary>
        protected RequestProcessorNodeOptions NodeOptions { get; }

        protected ILogger Logger { get; }
        protected ILoggerFactory LoggerFactory { get; }

        /// <summary>
        /// Диспетчер запросов.
        /// </summary>
        protected TDispatcher Dispatcher { get; private set; } = null!;

        /// <summary>
        /// Отдельные БД, к которым мы подключаемся для получения новых событий.
        /// </summary>
        protected IReadOnlyList<IEventSource> EventSources => EventReceivers.Keys.ToArray();

        /// <summary>
        /// Штука для уведомления о появлении в очереди новых событий, чтобы диспетчер мог раскидать запросы по воркероам.
        /// </summary>
        /// <remarks>
        /// Вынесен сюда отдельно, чтобы можно было периодически его сбрасывать, а то мало ли, события из БД не доходят до нас.
        /// </remarks>
        protected EventWaitHandle EventWaitHandle { get; }

        /// <inheritdoc cref="RequestProcessorBootstrapperBase{TRequest,TWorkerParams,TWorker,TDispatcher,TProcessingRequestInfo}"/>
        protected RequestProcessorBootstrapperBase(
            RequestProcessorNodeOptions nodeOptions,
            ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider)
        {
            NodeOptions = nodeOptions ?? throw new ArgumentNullException(nameof(nodeOptions));
            NodeOptions.AssertValid();

            LoggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
            ServiceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));

            Logger = loggerFactory.CreateLogger(GetType().Name) ?? throw new ArgumentNullException(nameof(loggerFactory));

            EventWaitHandle = new EventWaitHandle(false, EventResetMode.ManualReset);
            EventReceivers = new ConcurrentDictionary<IEventSource, IEventReceiver>();
        }

        /// <inheritdoc />
        public sealed override async Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogDebug($"Запуск {NodeOptions.Name}...");

            // получим список БД, за которым надо следить
            var eventSources = GetEventSources();

            // сбросим залипшие задачи
            await ResetStuckRequestsAsync(eventSources, cancellationToken);

            // создадим и запустим диспетчер запросов
            Dispatcher = CreateDispatcher(eventSources);
            await Dispatcher.StartAsync(cancellationToken);

            // запустим прослушивание событий БД
            var eventReceiverStartTasks = new List<Task>(eventSources.Count);
            foreach (var monitoredDatabase in eventSources)
            {
                eventReceiverStartTasks.Add(StartEventReceiverAsync(monitoredDatabase, cancellationToken));
            }
            await Task.WhenAll(eventReceiverStartTasks);

            // запустим главный поток - периодический опрос БД
            await base.StartAsync(cancellationToken);

            // установим событие, чтобы сразу что-то полезное сделать
            EventWaitHandle.Set();

            Logger.LogDebug($"{NodeOptions.Name} запущен");
        }

        /// <summary>
        /// Возвращает список БД, к которым надо будет подключиться.
        /// </summary>
        /// <remarks>
        /// В этом методе нужно реализовать формирование списка БД, события в которых мы будем слушать.
        /// </remarks>
        protected abstract IReadOnlyList<IEventSource> GetEventSources();

        /// <summary>
        /// Создаёт диспетчер обработки запросов.
        /// </summary>
        /// <remarks>
        /// В этом методе нужно создать и настроить диспетчера, который будет раскидывать запросы по воркерам.
        /// </remarks>
        protected abstract TDispatcher CreateDispatcher(IReadOnlyList<IEventSource> monitoredDatabases);

        /// <summary>
        /// Сбрасывает "залипшие" запросы (т.е. взятые в работу, но не сброшенные после остановки приложения).
        /// </summary>
        /// <remarks>
        /// В этом методе нужно реализовать логику по возврату запросов, которые в прошлые запуски брались в работу,
        /// но из-за внезапной ошибки не были корректны завершены и были возвращены обратно в очередь.
        /// </remarks>
        protected virtual Task ResetStuckRequestsAsync(IReadOnlyList<IEventSource> monitoredDatabases, CancellationToken cancellationToken = default)
        {
            // по умолчанию мы ничего не знаем о запросах, поэтому ничего и не сбрасываем
            // наследники класса переопределят, если надо
            // метод виртуальный, потому что ни везде надо делать сброс.

            return Task.CompletedTask;
        }

        /// <summary>
        /// Запускает прослушивание событий для указанной БД.
        /// </summary>
        /// <param name="monitoredDatabase">БД, события от которой надо слушать.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        protected abstract Task StartEventReceiverAsync(IEventSource monitoredDatabase, CancellationToken cancellationToken = default);

        /// <summary>
        /// Обрабатывает получения события ид БД.
        /// </summary>
        /// <remarks>
        /// Базовая реализация просто устанавливает событие, чтобы диспетчер мог взять запросы в работу.
        /// </remarks>
        protected virtual void HandleDbEventReceived(object sender, IRequestProcessingEvent e)
        {
            EventWaitHandle.Set();
        }

        /// <summary>
        /// С заданной периодичностью устанавливает <see cref="ManualResetEvent"/>
        /// для проверки наличия новых запросов в БД
        /// </summary>
        protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            Logger.LogDebug("Периодическая проверка очередей запущена");
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    Logger.LogTrace($"Ожидаем периодической проверки очередей через {NodeOptions.EventsPeriodicCheckSec} сек...");
                    await Task.Delay(TimeSpan.FromSeconds(NodeOptions.EventsPeriodicCheckSec), stoppingToken);

                    Logger.LogTrace("Выполняем периодическую проверку очередей...");
                    await PeriodicEventSourceCheckActionAsync(stoppingToken);
                    Logger.LogTrace("Периодическая проверка очередей завершена");
                }
                // Идет остановка, все под контролем
                catch (Exception) when (stoppingToken.IsCancellationRequested)
                {
                    Logger.LogDebug("Останавливаем периодическую проверку очередей...");
                }
                catch (Exception e)
                {
                    Logger.LogError(e, $"Ошибка периодической проверки очередей. Причина: {e.Message}");
                }
            }

            Logger.LogDebug("Периодическая проверка очередей остановлена");
        }

        /// <inheritdoc />
        public sealed override async Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.LogDebug($"Остановка {NodeOptions.Name}...");

            // остановим всех слушателей БД
            var dbEventListerStopTasks = new List<Task>();
            foreach (var monitoredDatabase in EventSources)
            {
                dbEventListerStopTasks.Add(StopListenEventSourceAsync(monitoredDatabase, cancellationToken));
            }
            await Task.WhenAll(dbEventListerStopTasks);

            // остановим главный поток - периодический опрос БД
            await base.StopAsync(cancellationToken);

            // сбросим ожидания
            EventWaitHandle.Reset();

            // остановим диспетчер запросов
            await Dispatcher.StopAsync(cancellationToken);

            Logger.LogDebug($"{NodeOptions.Name} остановлен");
        }

        /// <summary>
        /// Прекращает прослушивание событий от БД.
        /// </summary>
        /// <param name="eventSource">Источник событий.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <exception cref="InvalidOperationException"></exception>
        protected async Task StopListenEventSourceAsync(IEventSource eventSource, CancellationToken cancellationToken = default)
        {
            if (eventSource == null) throw new ArgumentNullException(nameof(eventSource));

            if (!EventReceivers.TryRemove(eventSource, out var eventReceiver))
                throw new InvalidOperationException($"БД {eventSource} не была добавлена в список и не прослушивается.");

            // остановимся
            await eventReceiver.StopAsync(cancellationToken);

            // отпишемся от событий
            eventReceiver.OnEventReceived -= HandleDbEventReceived;
        }

        /// <summary>
        /// Действие по периодической проверки событий в БД.
        /// </summary>
        /// <remarks>
        /// В базовой реализации просто устанавливает событие, чтобы выполнилась проверка БД.
        /// Можно переопределить метод, если нужны дополнительное действия.
        /// </remarks>
        protected virtual Task PeriodicEventSourceCheckActionAsync(CancellationToken stoppingToken = default)
        {
            EventWaitHandle.Set();

            return Task.CompletedTask;
        }

        /// <summary>
        /// Создаёт воркеров согласно настройкам.
        /// </summary>
        protected IReadOnlyList<TWorker> CreateWorkers(RequestProcessorNodeOptions workerOptions)
        {
            if (workerOptions == null) throw new ArgumentNullException(nameof(workerOptions));

            // сделаем локальный логер, чтобы логировать процесс создания групп
            var logger = LoggerFactory.CreateLogger(GetType().Name);

            logger.LogDebug("Создаём воркеров...");

            var workers = new List<TWorker>(workerOptions.WorkersCount);
            for (var i = 0; i < workerOptions.WorkersCount; i++)
            {
                // сделаем каждому воркеру свой логер, чтобы логи каждого воркера были в отдельном файле
                var workerName = $"worker_{i}";
                var workerLogger = LoggerFactory.CreateLogger(workerName);

                // сформируем параметры для воркера
                var createWorkerParams = CreateWorkerParams(workerLogger);

                // и создадим воркер
                logger.LogDebug($"Создаём worker с именем \"{workerName}\"...");
                workers.Add(CreateWorker(createWorkerParams));
            }

            logger.LogInformation("Создание воркеров завершено");

            return workers;
        }

        /// <summary>
        /// Фабричный метод для создания параметров воркера <see cref="TWorkerParams"/>, которые он не сможем получить из DI (например, логер, созданный с именем воркера)
        /// </summary>
        protected abstract TWorkerParams CreateWorkerParams(ILogger logger);

        /// <summary>
        /// Фабричный метод для создания воркеров.
        /// </summary>
        /// <param name="createWorkerParams">Экстра параметры, которые надо передать воркеру и которые он не сможет взять из DI</param>
        protected virtual TWorker CreateWorker(TWorkerParams createWorkerParams)
        {
            var worker = ServiceProvider.GetRequiredService<TWorker>();

            worker.Init(createWorkerParams);

            return worker;
        }
    }
}
