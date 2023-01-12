namespace Curiosity.Tools
{
    /// <summary>
    /// Wrapper of DateTime
    /// </summary>
    public class LocalDateTimeService : IDateTimeService
    {
        /// <inheritdoc />
        public System.DateTime GetCurrentTimeUtc()
        {    
            return System.DateTime.UtcNow;
        }
    }
}