namespace Curiosity.RequestProcessing
{
    /// <summary>
    /// Информация о поступившем событии.
    /// </summary>
    public interface IRequestProcessingEvent
    {
        /// <summary>
        /// Название события/канала уведомления.
        /// </summary>
        public string EventName { get; }

        /// <summary>
        /// Аргументы события.
        /// </summary>
        public string? Payload { get; }
    }
}
