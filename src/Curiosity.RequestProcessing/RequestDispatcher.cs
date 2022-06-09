using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.RequestProcessing.Workers;
using Curiosity.Tools;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Curiosity.RequestProcessing
{
    /// <summary>
    /// Базовый класс диспетчера обработки запросов из очереди. Распределяет запросы по нужным воркерам.
    /// </summary>
    /// <typeparam name="TRequest">Тип запроса для обработки (POCO класс)</typeparam>
    /// <typeparam name="TWorker">Тип воркера, который используется в обработке</typeparam>
    /// <typeparam name="TWorkerExtraParams">Параметры для <see cref="TWorker"/>.</typeparam>
    /// <typeparam name="TProcessingRequestInfo">Информация о запросе, который воркер обрабатывает в данный момент.</typeparam>
    /// <typeparam name="TOptions">Общие параметры обработчика запросов.</typeparam>
    public abstract class RequestDispatcherBase<TRequest, TWorker, TWorkerExtraParams, TProcessingRequestInfo, TOptions> : BackgroundService
        where TRequest : IRequest
        where TWorkerExtraParams : IWorkerExtraParams
        where TWorker : WorkerBase<TRequest, TWorkerExtraParams, TProcessingRequestInfo, TOptions>
        where TProcessingRequestInfo : class, IProcessingRequestInfo
        where TOptions: RequestProcessorNodeOptions
    {
        /// <summary>
        /// Периодичность записи в лог текущего состояния диспетчера (сколько воркеров загружено).
        /// </summary>
        private readonly TimeSpan _stateFlushPeriod;

        /// <summary>
        /// Общие параметры обработчика запросов.
        /// </summary>
        protected TOptions NodeOptions { get; }

        /// <summary>
        /// Имя приложения-обработчика.
        /// </summary>
        protected string NodeName => NodeOptions.Name;

        /// <summary>
        /// Событие, по наступлению которого необходимо по новой проверить наличие запросов в очереди.
        /// </summary>
        /// <remarks>
        /// Либо пришло событие из базы, либо по таймеру проверяем.
        /// </remarks>
        protected EventWaitHandle NewEventWaitHandle { get; }

        /// <summary>
        /// Логер.
        /// </summary>
        protected ILogger Logger { get; }

        /// <summary>
        /// Воркеры.
        /// </summary>
        protected IReadOnlyList<TWorker> Workers { get; }

        /// <inheritdoc cref="RequestDispatcherBase{TRequest,TWorker,TWorkerExtraParams,TProcessingRequestInfo,TOptions}"/>
        protected RequestDispatcherBase(
            TOptions nodeOptions,
            EventWaitHandle manualResetEvent,
            IReadOnlyList<TWorker> workers,
            ILogger logger)
        {
            NodeOptions = nodeOptions ?? throw new ArgumentNullException(nameof(nodeOptions));
            NewEventWaitHandle = manualResetEvent ?? throw new ArgumentNullException(nameof(manualResetEvent));

            Logger = logger ?? throw new ArgumentNullException(nameof(logger));

            Workers = workers ?? throw new ArgumentNullException(nameof(workers));
            if (workers.Count == 0) throw new ArgumentException("Нужно указать хотя бы одного воркера для обработки", nameof(workers));

            _stateFlushPeriod = TimeSpan.FromSeconds(nodeOptions.StateFlushPeriodSec);
        }

        /// <summary>
        /// Занимается распределение запросов по воркерам.
        /// </summary>
        /// <remarks>
        /// Ждет появления задачи в очереди, получает ее из базы и отдает воркеру <see cref="TWorker"/>.
        /// </remarks>
        protected sealed override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            // определим тип события, по которым воркер должен просыпаться и что-то делать
            var waitEventHandles = new[]
            {
                NewEventWaitHandle,
                stoppingToken.WaitHandle
            };

            var sw = new Stopwatch();
            var lastStateFlushTimestamp = DateTime.UtcNow;
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // 0. Ждем наступления события, когда надо проверить очередь на наличие новых запросов
                    Logger.LogTrace("Ожидаем получения событий из очереди...");
                    WaitHandle.WaitAny(waitEventHandles);

                    stoppingToken.ThrowIfCancellationRequested();

                    NewEventWaitHandle.Reset();

                    // дополнительно выведем в лог загрузку воркеров
                    var now = DateTime.UtcNow;
                    if (now - lastStateFlushTimestamp > _stateFlushPeriod)
                    {
                        var busyWorkersCount = Workers.Count(x => x.IsBusy);
                        Logger.LogInformation($"{busyWorkersCount}/{Workers.Count} воркеров загружены (загрузка сервиса {(decimal)busyWorkersCount/Workers.Count:P})");
                        lastStateFlushTimestamp = now;
                    }

                    // 1. Получим свободных воркеров
                    var freeWorkers = Workers.Where(x => !x.IsBusy).ToArray();
                    Logger.LogTrace($"{freeWorkers.Length} воркеров доступно для обработки запросов");
                    if (freeWorkers.Length == 0) continue;

                    // 2. Запрашиваем из БД запросы, удовлетворяющие фильтру и количеству свободных воркеров
                    sw.Restart();
                    var requests = await GetRequestsAsync(
                        freeWorkers.Length,
                        stoppingToken);
                    sw.Stop();

                    Logger.LogTrace($"{requests?.Count ?? 0} запросов получено из очереди за {sw.ElapsedMilliseconds} мс");
                    if (requests == null || requests.Count == 0) continue;

                    // 3. раскидываем полученные запросы по воркерам
                    var requestIndex = 0;
                    Logger.LogTrace("Распределяем запросы по свободным воркерам...");
                    for (var index = 0; index < freeWorkers.Length; index++)
                    {
                        if (requestIndex == requests.Count) break;

                        var worker = freeWorkers[index];
                        var request = requests[requestIndex];

                        worker.ProcessRequestSafelyAsync(request, stoppingToken).WithCompletion(t =>
                        {
                            // после обработки запроса надо сбросить event, чтобы сразу проверить наличие новых запросов в очередях
                            NewEventWaitHandle.Set();

                            HandleRequestProcessingCompletionAsync(request, t.IsCompletedSuccessfully, stoppingToken).WithExceptionLogger(Logger);
                        });
                        await HandleRequestProcessingStartedAsync(request, stoppingToken);

                        requestIndex++;
                    }

                    // если после того, как раскидали запросы, есть еще свободные воркеры, попробуем раскидать дальше
                    if (freeWorkers.Length > requests.Count)
                        NewEventWaitHandle.Set();
                }
                // Идет остановка - все под контролем
                catch (Exception ex) when (stoppingToken.IsCancellationRequested)
                {
                    Logger.LogWarning(ex, $"Поймали исключение при остановке диспетчера. Исключение: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, $"Необработанное исключение при распределении запросов воркерам. Исключение: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Возвращает запросы для обработки.
        /// </summary>
        /// <param name="maxRequestsCount">
        ///     Максимальное количество запросов, которое можно вернуть.
        ///     Нужно, чтобы запрашивать запросы только под свободные воркеры.
        /// </param>
        /// <param name="cancellationToken">Токен отмены.</param>
        /// <remarks>
        /// Нужно переопределить в дочернем диспетчере этот метод, чтобы в нем реализовать логику запроса событий из БД.
        /// </remarks>
        protected abstract Task<IReadOnlyList<TRequest>?> GetRequestsAsync(
            int maxRequestsCount,
            CancellationToken cancellationToken = default);

        /// <summary>
        /// Позволяет обработать запрос до того, как воркер начал работу с ним. 
        /// </summary>
        /// <param name="request">Запрос.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        protected virtual Task HandleRequestProcessingStartedAsync(TRequest request, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Позволяет обработать запрос после того, как воркер закончил работу с ним. 
        /// </summary>
        /// <param name="request">Запрос.</param>
        /// <param name="isSuccessful">Результат обработку.</param>
        /// <param name="cancellationToken">Токен отмены.</param>
        protected virtual Task HandleRequestProcessingCompletionAsync(TRequest request, bool isSuccessful, CancellationToken cancellationToken = default)
        {
            return Task.CompletedTask;
        }
    }
}
