using Curiosity.Configuration;

namespace Curiosity.Hosting
{
    /// <summary>
    /// Logging configuration file options.
    /// </summary>
    public interface ILoggingConfigurationOptions : 
        ILoggableOptions,
        IValidatableOptions
    {
        /// <summary>
        /// Absolute path to logging configuration file.
        /// </summary>
        /// <remarks>
        /// For example, path to NLog.config
        /// </remarks>
        string LogConfigurationPath { get; }
        
        /// <summary>
        /// Absolute path to log output directory.
        /// </summary>
        string LogOutputDirectory { get; }
    }
}