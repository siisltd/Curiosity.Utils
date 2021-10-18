namespace Curiosity.RequestProcessing
{
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