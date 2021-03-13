using System.Collections.Generic;
using Curiosity.Configuration;
using Curiosity.Hosting.ThreadPool;

namespace Curiosity.Hosting.Web
{
    /// <inheritdoc cref="ICuriosityWebAppConfiguration" />
    public abstract class CuriosityWebAppConfiguration : CuriosityAppConfiguration, ICuriosityWebAppConfiguration
    {
        /// <inheritdoc />
        public string? Urls { get; set; }

        /// <inheritdoc />
        public KestrelOptions? Kestrel { get; set; }

        /// <inheritdoc />
        public ThreadPoolOptions ThreadPool { get; set; } = new ThreadPoolOptions();
        
        /// <inheritdoc />
        public string[]? SensitiveDataFieldNames { get; set; }

        /// <inheritdoc />
        public override IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
        {
            return CuriosityWebAppConfigurationValidator.Validate(this, prefix);
        }
    }
}