using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Curiosity.RequestProcessing.Workers
{
    /// <summary>
    /// Базовый класс обработчика запросов из очереди. Он получает запрос <typeparam name="TRequest"/> и обрабатывает его.
    /// </summary>
    /// <typeparam name="TRequest">Тип запроса</typeparam>
    /// <typeparam name="TWorkerParams">Доп. параметры для воркера.</typeparam>
    /// <typeparam name="TProcessingRequestInfo">Информация о запросе, который воркер обрабатывает в данный момент.</typeparam>
    /// <typeparam name="TOptions">Общие параметры обработчика запросов.</typeparam>
    public abstract class WorkerBase<TRequest, TWorkerParams, TProcessingRequestInfo, TOptions>
        where TRequest : IRequest
        where TWorkerParams : IWorkerExtraParams
        where TProcessingRequestInfo : class, IProcessingRequestInfo
        where TOptions: RequestProcessorNodeOptions
    {
        /// <summary>
        /// Воркер в данный момент обрабатывает новый запрос?
        /// </summary>
        /// <remarks>
        /// Можно переопределить, чтобы реализовать свою логику определения занятости воркера.
        /// </remarks>
        public virtual bool IsBusy => ProcessingRequest != null;

        /// <summary>
        /// Информация о запросе, который сейчас обрабатывается воркером.
        /// </summary>
        public TProcessingRequestInfo? ProcessingRequest { get; private set; }

        /// <summary>
        /// Логер воркера.
        /// </summary>
        /// <exception cref="InvalidOperationException"></exception>
        protected ILogger Logger
        {
            get
            {
                if (_logger == null)
                    throw new InvalidOperationException($"{nameof(WorkerBase<TRequest, TWorkerParams, TProcessingRequestInfo, TOptions>)} is not initialized. Call {nameof(Init)} first");
                
                return _logger;
            }
        }
        private ILogger? _logger;

        /// <summary>
        /// Общие параметры обработчика запросов.
        /// </summary>
        protected TOptions NodeOptions { get; }

        /// <inheritdoc cref="WorkerBase{TRequest,TWorkerParams,TProcessingRequestInfo,TOptions}"/>
        protected WorkerBase(TOptions nodeOptions)
        {
            NodeOptions = nodeOptions ?? throw new ArgumentNullException(nameof(nodeOptions));
        }

        /// <summary>
        /// Инициализирует воркер указанными параметрами.
        /// </summary>
        public virtual void Init(TWorkerParams workerParams)
        {
            if (workerParams == null) throw new ArgumentNullException(nameof(workerParams));

            _logger = workerParams.Logger;
        }
        
        /// <summary>
        /// Выполняет обработку запроса.
        /// </summary>
        /// <remarks>
        /// Нужно реализовать в этом методе логику обработки запроса.
        /// </remarks>
        protected abstract Task ProcessRequestAsync(TRequest request, CancellationToken cancellationToken);

        /// <summary>
        /// Обрабатывает неперехваченное исключение, которое было выброшено в <see cref="ProcessRequestAsync"/>
        /// </summary>
        /// <param name="request">Запрос, при обработке которого возникло исключение</param>
        /// <param name="ex">Сами исключение</param>
        /// <remarks>
        /// Нужно реализовать в этом методе логику обработки исключения.
        /// </remarks>
        protected abstract Task HandleExceptionAsync(TRequest request, Exception ex);

        /// <summary>
        /// Выполняет "безопасную" обработку запроса.
        /// </summary>
        /// <remarks>
        /// Вызов <see cref="ProcessRequestAsync"/> с гарантированным перехватом исключения и установкой статуса.
        /// </remarks>
        public async Task ProcessRequestSafelyAsync(TRequest request, CancellationToken cancellationToken)
        {
            // сразу пометим, что воркер занят, т.к. может возникнуть гонка потоков,
            // и один воркер сможет случайно взять два запроса в обработку
            ProcessingRequest = GetRequestInfo(request);
            
            // отпустим родительский поток, чтобы работа диспетчера не блокировалась
            await Task.Yield();
            
            // проставим культуру, в которой должен обрабатываться запрос
            var culture = request.RequestCulture;
            Thread.CurrentThread.CurrentCulture = culture;
            Thread.CurrentThread.CurrentUICulture = culture;

            // а теперь можно обрабатывать запрос
            try
            {
                await ProcessRequestAsync(request, cancellationToken);
            }
            catch (Exception ex)
            {
                // сюда сознательно не передаём токен отмены,
                // чтобы воркер корректно завершил свою работу в случае ошибки
                await HandleExceptionAsync(request, ex);
            }
            finally
            {
                // напоследок пометим воркер как свободный, чтобы он мог брать в работу новые запросы
                ProcessingRequest = null;
            }
        }

        /// <summary>
        /// Возвращает информацию о текущем запроса.
        /// </summary>
        protected abstract TProcessingRequestInfo GetRequestInfo(TRequest request);
    }
}
