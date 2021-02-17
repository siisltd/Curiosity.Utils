using System;
using System.IO;
using Curiosity.Configuration;
using Microsoft.Extensions.Configuration;

namespace Curiosity.Configurations
{
    /// <summary>
    /// Provides configuration from YAML files.
    /// </summary>
    /// <typeparam name="T">POCO configuration.</typeparam>
    public class YamlConfigurationProvider<T> : ConfigurationProviderBase<T> where T : class, new()
    {
        public YamlConfigurationProvider(string? configurationBasePath, bool isConfigOptional = false): base(configurationBasePath, isConfigOptional)
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
                
            // check main configuration file existence because we want to throw more obvious exception 
            if (!isFileOptional)
            {
                var pathToMainConfiguration = Path.Combine(PathToConfigurationFiles, file);
                if (!File.Exists(file))
                    throw new InvalidOperationException($"Configuration file \"{Path.GetFileName(file)}\" does not exist in directory \"{Path.GetDirectoryName(pathToMainConfiguration)}\"");

            }
            
            configurationBuilder.AddYamlFile(file, isFileOptional);
        }
    }
}