using System;

namespace Curiosity.RequestProcessing.Workers
{
    /// <summary>
    /// Информация о запросе, который выполняется в данный момент.
    /// </summary>
    public interface IProcessingRequestInfo
    {
        /// <summary>
        /// Время, когда запрос взяли в обработку.
        /// </summary>
        public DateTime ProcessingStarted { get; }
    }
}
