using System;
using Microsoft.Extensions.Logging;

namespace Curiosity.RequestProcessing.Workers
{
    /// <summary>
    /// Доп.параметры для любого воркера, которые он не сможет получить из DI.
    /// </summary>
    /// <remarks>
    /// Например, именованный логер или фильтры для выборки запросов из БД согласно настройкам группы.
    /// </remarks>
    public class WorkerBasicExtraParams : IWorkerExtraParams
    {
        /// <inheritdoc />
        public ILogger Logger { get; }

        /// <inheritdoc cref="WorkerBasicExtraParams"/>
        public WorkerBasicExtraParams(ILogger logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
    }
}
