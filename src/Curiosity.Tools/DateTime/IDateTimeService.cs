namespace Curiosity.Tools
{
    /// <summary>
    /// Service for retrieving current date and time.
    /// </summary>
    public interface IDateTimeService
    {
        /// <summary>
        /// Returns actual date and time in UTC
        /// </summary>
        /// <returns>UTC date and time</returns>
        System.DateTime GetCurrentTimeUtc();
    }
}