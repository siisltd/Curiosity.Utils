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

        public ThreadPoolOptions ThreadPool { get; set; } = new ThreadPoolOptions();

        /// <inheritdoc />
        public override IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
        {
            return CuriosityWebAppConfigurationValidator.Validate(this, prefix);
        }
    }
}