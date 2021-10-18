using System;

namespace SIISLtd.RequestProcessing.Workers
{
    /// <summary>
    /// Информация о запросе, который выполняется в данный момент.
    /// </summary>
    public class ProcessingRequestInfo
    {
        /// <summary>
        /// Время, когда запрос взяли в обработку.
        /// </summary>
        public DateTime ProcessingStarted { get; }

        /// <summary>
        /// ИД проекта, к которому относится запрос.
        /// </summary>
        public long ProjectId { get; }

        /// <summary>
        /// ИД клиента, которому принадлежит проект.
        /// </summary>
        public long ClientId { get; }

        public ProcessingRequestInfo(
            long projectId,
            long clientId,
            DateTime processingStarted)
        {
            ProjectId = projectId;
            ClientId = clientId;
            ProcessingStarted = processingStarted;
        }
    }
}
