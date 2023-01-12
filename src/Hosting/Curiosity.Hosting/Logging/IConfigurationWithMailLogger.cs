using Curiosity.Configuration;

namespace Curiosity.Hosting
{
    /// <summary>
    /// Configuration that contains EMail options for logger.
    /// </summary>
    public interface IConfigurationWithMailLogger : ILoggableOptions, IValidatableOptions
    {
        /// <summary>
        /// EMail options for logger.
        /// </summary>
        ILoggerMailOptions LoggerMail { get; }
    }
}