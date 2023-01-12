using Microsoft.Extensions.Configuration;

namespace Curiosity.Configuration
{
    /// <summary>
    /// Provides configuration from YAML files.
    /// </summary>
    /// <typeparam name="T">POCO configuration.</typeparam>
    public class YamlConfigurationProvider<T> : ConfigurationProviderBase<T> where T : class, new()
    {
        public YamlConfigurationProvider(string? configurationBasePath, bool isConfigOptional = false, string[]? cliArgs = null): base(configurationBasePath, isConfigOptional, cliArgs)
        {
        }
        
        /// <inheritdoc />
        protected override void SetBasePath(IConfigurationBuilder configurationBuilder, string basePath)
        {
            configurationBuilder.SetBasePath(basePath);
        }

        /// <inheritdoc />
        protected override void AddFile(IConfigurationBuilder configurationBuilder, string fileNameWithoutExtension, bool isFileOptional)
        {
            var file = $"{fileNameWithoutExtension}.yml";
            
            configurationBuilder.AddYamlFile(file, isFileOptional);
        }
    }
}
