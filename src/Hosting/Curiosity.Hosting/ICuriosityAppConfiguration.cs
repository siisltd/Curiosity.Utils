using Curiosity.Configuration;
using Curiosity.Tools;

namespace Curiosity.Hosting
{
    /// <summary>
    /// Basic configuration for Marvin's app.
    /// </summary>
    public interface ICuriosityAppConfiguration: IValidatableOptions, ILoggableOptions
    {
        /// <summary>
        /// Application name.
        /// </summary>
        /// <remarks>
        /// Uses in logging, etc.
        /// </remarks>
        public string AppName { get; }

        /// <summary>
        /// Culture options.
        /// </summary>
        public CultureOptions Culture { get; }
        
        /// <summary>
        /// Logs configuration options.
        /// </summary>
        public ILoggingConfigurationOptions Log { get; }
    }
}