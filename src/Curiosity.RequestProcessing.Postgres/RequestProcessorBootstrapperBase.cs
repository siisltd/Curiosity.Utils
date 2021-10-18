using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SIISLtd.RequestProcessing.Options;
using SIISLtd.RequestProcessing.Workers;

namespace SIISLtd.RequestProcessing
{
    /// <summary>
    /// Бустрапер сервисов обработки запросов: подключается к БД, слушает события и управляет жизненным циклом диспетчера обработки запросов, а также созаёт воркеры.
    /// </summary>
    /// <typeparam name="TRequest">Тип POCO класса запроса, который будет обрабатывать <see cref="TWorker"/>.</typeparam>
    /// <typeparam name="TRequestEntity">Тип Entity сущности запроса, который будет доставаться из БД. Нужен для того, чтобы работала фильтрация запросов.</typeparam>
    /// <typeparam name="TWorker">Тип воркера, который будет обрабатывать запросы.</typeparam>
    /// <typeparam name="TDispatcher">Тип диспетчера, который будет раскидывать запросы <see cref="TRequest"/> по воркерам <see cref="TWorker"/>.</typeparam>
    public abstract class RequestProcessorBootstrapperBase<TRequest, TRequestEntity, TWorker, TDispatcher> : RequestProcessorBootstrapperBase<TRequest, TRequestEntity, WorkerBasicExtraParams, TWorker, TDispatcher, ProcessingRequestInfo>
        where TRequest : IRequest
        where TWorker : WorkerBase<TRequest, WorkerBasicExtraParams, ProcessingRequestInfo>
        where TDispatcher : RequestDispatcherBase<TRequest, TRequestEntity, TWorker, WorkerBasicExtraParams, ProcessingRequestInfo>, IHostedService
    {
        protected RequestProcessorBootstrapperBase(
            RequestProcessorNodeOptions nodeOptions,
            ILoggerFactory loggerFactory,
            IServiceProvider serviceProvider) : base(nodeOptions, loggerFactory, serviceProvider)
        {
        }

        protected override WorkerBasicExtraParams CreateWorkerParams(ILogger logger)
        {
            return new WorkerBasicExtraParams(logger);
        }
    }

    /// <summary>
    /// Бустрапер сервисов обработки запросов: подключается к БД, слушает события и управляет жизненным циклом диспетчера обработки запросов, а также созаёт воркеры.
    /// </summary>
    /// <typeparam name="TRequest">Тип POCO класса запроса, который будет обрабатывать <see cref="TWorker"/>.</typeparam>
    /// <typeparam name="TRequestEntity">Тип Entity сущности запроса, который будет доставаться из БД. Нужен для того, чтобы работала фильтрация запросов.</typeparam>
    /// <typeparam name="TWorkerParams">Параметры для создания воркера, которые нельзя получить из DI (например, логгер, созданный с именем воркера)</typeparam>
    /// <typeparam name="TWorker">Тип воркера, который будет обрабатывать запросы.</typeparam>
    /// <typeparam name="TDispatcher">Тип диспетчера, который будет раскидывать запросы <see cref="TRequest"/> по воркерам <see cref="TWorker"/>.</typeparam>
    /// <typeparam name="TProcessingRequestInfo">Информация о запросе, который воркер обрабатывает в данный момент.</typeparam>
    public abstract class RequestProcessorBootstrapperBase<TRequest, TRequestEntity, TWorkerParams, TWorker, TDispatcher, TProcessingRequestInfo> : BackgroundService
        where TRequest : IRequest
        where TWorker : WorkerBase<TRequest, TWorkerParams, TProcessingRequestInfo>
        where TWorkerParams : IWorkerExtraParams
        where TDispatcher: RequestDispatcherBase<TRequest, TRequestEntity, TWorker, TWorkerParams, TProcessingRequestInfo>, IHostedService
        where TProcessingRequestInfo : ProcessingRequestInfo
    {
        /// <summary>
        /// Слушатели событий в БД (можно одновременно слушать события из разных БД).
        /// </summary>
        private readonly ConcurrentDictionary<MonitoredDatabase, DbEventListener> _dbEventListeners;

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
        protected IReadOnlyList<MonitoredDatabase> MonitoredDatabase => _dbEventListeners.Keys.ToArray();

        /// <summary>
        /// Штука для уведомления о появлении в очереди новых событий, чтобы диспетчер мог раскидать запросы по воркероам.
        /// </summary>
        /// <remarks>
        /// Вынесен сюда отдельно, чтобы можно было периодически его сбрасывать, а то мало ли, события из БД не доходят до нас.
        /// </remarks>
        protected EventWaitHandle EventWaitHandle { get; }

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
            _dbEventListeners = new ConcurrentDictionary<MonitoredDatabase, DbEventListener>();
        }

        public sealed override async Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogDebug($"Запуск {NodeOptions.Name}...");

            // получим список БД, за которым надо следить
            var monitoredDatabases = GetMonitoredDatabases();

            // сбросим залипшие задачи
            await ResetStuckRequestsAsync(monitoredDatabases, cancellationToken);

            // создадим и запустим диспетчер запросов
            Dispatcher = CreateDispatcher(monitoredDatabases);
            await Dispatcher.StartAsync(cancellationToken);

            // запустим прослушивание событий БД
            var dbEventListenerStartTasks = new List<Task>();
            foreach (var monitoredDatabase in monitoredDatabases)
            {
                dbEventListenerStartTasks.Add(StartListenDbAsync(monitoredDatabase, cancellationToken));
            }
            await Task.WhenAll(dbEventListenerStartTasks);

            // запустим главный поток - периодический опрос БД
            await base.StartAsync(cancellationToken);

            // просетим событие, чтобы сразу что-то полезное сделать
            EventWaitHandle.Set();

            Logger.LogDebug($"{NodeOptions.Name} запущен");
        }

        /// <summary>
        /// Возвращает список БД, к которым надо будет подключиться.
        /// </summary>
        /// <remarks>
        /// В этом методе нужно реализовать формирование списка БД, события в которых мы будем слушать.
        /// </remarks>
        protected abstract IReadOnlyList<MonitoredDatabase> GetMonitoredDatabases();

        /// <summary>
        /// Создаёт диспетчер обработки запросов.
        /// </summary>
        /// <remarks>
        /// В этом методе нужно создать и настроить диспетчера, который будет раскидывать запросы по воркерам.
        /// </remarks>
        protected abstract TDispatcher CreateDispatcher(IReadOnlyList<MonitoredDatabase> monitoredDatabases);

        /// <summary>
        /// Сбрасывает "залипшие" запросы (т.е. взятые в работу, но не сброшенные после остановки приложения).
        /// </summary>
        /// <remarks>
        /// В этом методе нужно реализовать логику по возврату запросов, которые в прошлые запуски брались в работу,
        /// но из-за внезапной ошибки не были корректны завершены и были возвращены обратно в очередь.
        /// </remarks>
        protected virtual Task ResetStuckRequestsAsync(IReadOnlyList<MonitoredDatabase> monitoredDatabases, CancellationToken cancellationToken = default)
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
        protected async Task StartListenDbAsync(MonitoredDatabase monitoredDatabase, CancellationToken cancellationToken = default)
        {
            if (monitoredDatabase == null) throw new ArgumentNullException(nameof(monitoredDatabase));

            if (_dbEventListeners.TryGetValue(monitoredDatabase, out _))
                throw new InvalidOperationException($"БД {monitoredDatabase} уже добавлена в список и прослушивается.");

            var dbEventListenerLogger = LoggerFactory.CreateLogger<DbEventListener>();

            var dbEventListener = new DbEventListener(
                monitoredDatabase,
                NodeOptions,
                dbEventListenerLogger,
                NodeOptions.EventNames);

            // подпишемся на события в этой БД и запустим слушателя
            dbEventListener.OnEventReceived += HandleDbEventReceived;

            await dbEventListener.StartAsync(cancellationToken);

            // добавим слушателя в список, чтобы потом корректно завершить работу
            _dbEventListeners[monitoredDatabase] = dbEventListener;
        }

        /// <summary>
        /// Обрабатывает получения события ид БД.
        /// </summary>
        /// <remarks>
        /// Базовая реализация просто сетит событие, чтобы диспетчер мог взять запросы в работу.
        /// </remarks>
        protected virtual void HandleDbEventReceived(object sender, DbEventReceivedArgs e)
        {
            EventWaitHandle.Set();
        }

        /// <summary>
        /// С заданной периодичностью сэтит <see cref="ManualResetEvent"/>
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
                    await PeriodicDbCheckActionAsync(stoppingToken);
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

        public sealed override async Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.LogDebug($"Остановка {NodeOptions.Name}...");

            // остановим всех слушателей БД
            var dbEventListerStopTasks = new List<Task>();
            foreach (var monitoredDatabase in MonitoredDatabase)
            {
                dbEventListerStopTasks.Add(StopListenDbAsync(monitoredDatabase, cancellationToken));
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

        protected async Task StopListenDbAsync(MonitoredDatabase database, CancellationToken cancellationToken = default)
        {
            if (database == null) throw new ArgumentNullException(nameof(database));

            if (!_dbEventListeners.TryRemove(database, out var dbEventListener))
                throw new InvalidOperationException($"БД {database} не была добавлена в список и не прослушивается.");

            // остановимся
            await dbEventListener.StopAsync(cancellationToken);

            // отпишемся от событий
            dbEventListener.OnEventReceived -= HandleDbEventReceived;
        }

        /// <summary>
        /// Действие по периодической проверки событий в БД.
        /// </summary>
        /// <remarks>
        /// В базовой реализации просто сетит событие, чтобы выполнилась проверка БД.
        /// Можно переопределить метод, если нужны дополнительное действия.
        /// </remarks>
        protected virtual Task PeriodicDbCheckActionAsync(CancellationToken stoppingToken = default)
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
                // сделаем каждому воркеру свой логгер, чтобы логи каждого воркера были в отдельном файле
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
        /// Фабричный метод для создания параметров воркера <see cref="TWorkerParams"/>, которые он не сможем получить из DI (например, логгер, созданный с именем воркера)
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
