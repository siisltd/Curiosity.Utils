using System;

namespace Curiosity.RequestProcessing.Workers
{
    public interface IProcessingRequestInfo
    {
        /// <summary>
        /// Время, когда запрос взяли в обработку.
        /// </summary>
        public DateTime ProcessingStarted { get; }
    }
}
