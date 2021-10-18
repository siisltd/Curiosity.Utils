using System;
using Microsoft.Extensions.Logging;

namespace SIISLtd.RequestProcessing.Workers
{
    /// <summary>
    /// Доп.параметры для любого воркера, которые он не сможет получить из DI.
    /// </summary>
    /// <remarks>
    /// Например, именнованный логгер или фильтры для выборки запросов из БД согласно настройкам группы.
    /// </remarks>
    public class WorkerBasicExtraParams : IWorkerExtraParams
    {
        /// <inheritdoc />
        public ILogger Logger { get; }

        public WorkerBasicExtraParams(ILogger logger)
        {
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
    }
}
