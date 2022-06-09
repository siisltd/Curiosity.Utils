using Microsoft.Extensions.Logging;

namespace Curiosity.RequestProcessing.Workers
{
    /// <summary>
    /// Доп. параметры для воркера, которые он не может получить из DI (например, настроенный логер с его именем).
    /// </summary>
    public interface IWorkerExtraParams
    {
        /// <summary>
        /// Именованный логер.
        /// </summary>
        /// <remarks>
        /// Чтобы каждый воркер мог писать в свой личный лог файл.
        /// </remarks>
        ILogger Logger { get; }
    }
}
