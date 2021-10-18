using System;

namespace SIISLtd.RequestProcessing
{
    /// <summary>
    /// Аргументы события получения уведомления от БД, на которую мы подписаны.
    /// </summary>
    public class DbEventReceivedArgs
    {
        /// <summary>
        /// Информация о БД.
        /// </summary>
        public MonitoredDatabase DatabaseInfo { get; }

        /// <summary>
        /// Название события/канала уведомления.
        /// </summary>
        public string EventName { get; }

        /// <summary>
        /// Аргументы события.
        /// </summary>
        public string? Payload { get; }

        public DbEventReceivedArgs(
            MonitoredDatabase databaseInfo,
            string eventName,
            string? payload)
        {
            DatabaseInfo = databaseInfo ?? throw new ArgumentNullException(nameof(databaseInfo));
            if (String.IsNullOrWhiteSpace(eventName))
                throw new ArgumentNullException(nameof(eventName));

            EventName = eventName;
            Payload = payload;
        }
    }
}
