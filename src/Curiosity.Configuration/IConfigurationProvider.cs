using Microsoft.Extensions.Configuration;

namespace Curiosity.Configuration
{
    /// <summary>
    /// Provider of specified configurations.
    /// </summary>
    /// <remarks>
    /// Binds configuration section to POCO class.
    /// </remarks>
    public interface IConfigurationProvider<out T>
    {
        /// <summary>
        /// Path to directory where configurations are located.
        /// </summary>
        string PathToConfigurationFiles { get; }
        
        /// <summary>
        /// Returns raw (.NET Core key-value) configuration.
        /// </summary>
        /// <returns>Raw configuration.</returns>
        IConfiguration GetRawConfiguration();

        /// <summary>
        /// Returns strong typed configuration.
        /// </summary>
        /// <returns>Strong typed configuration (POCO class).</returns>
        T GetConfiguration();
        
        /// <summary>
        /// Configures specified app configuration.
        /// </summary>
        /// <param name="configuration">Instance of <see cref="IConfigurationBuilder"/> to configure.</param>
        void ConfigureAppConfiguration(IConfigurationBuilder configuration);
    }
}