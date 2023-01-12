using System.Collections.Generic;
using Curiosity.Configuration;
using Curiosity.Tools;

namespace Curiosity.Hosting
{
    /// <inheritdoc />
    public class CuriosityAppConfiguration : ICuriosityAppConfiguration
    {
        /// <inheritdoc />
        public string AppName { get; set; } = null!;

        /// <inheritdoc />
        public CultureOptions Culture { get; set; } = new CultureOptions();
        
        /// <inheritdoc />
        public ILoggingConfigurationOptions Log { get; set; } = new LoggingConfigurationOptions();

        /// <inheritdoc />
        public virtual IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
        {
            return CuriosityAppConfigurationValidator.Validate(this, prefix);
        }
    }
}