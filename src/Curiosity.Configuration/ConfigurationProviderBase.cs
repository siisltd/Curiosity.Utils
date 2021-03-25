using System;
using System.Collections.Generic;
using System.IO;
using Microsoft.Extensions.Configuration;

namespace Curiosity.Configuration
{
    /// <summary>
    /// Base class for all configuration providers.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class ConfigurationProviderBase<T>: IConfigurationProvider<T> where T: class, new()
    {
        private readonly bool _isConfigOptional;
        private const string ProductionEnvironmentName = "Production";
        
        private const string DefaultConfigurationFileName = "config";
        private const string EnvironmentConfigurationFileNameTemplate = "config.{0}";
        private const string EnvironmentAndUserConfigurationFileNameTemplate = "config.{0}.{1}";
        
        private IConfiguration? _configuration;
        private T? _typedConfiguration;
        private readonly string[]? _cliArgs;

        /// <inheritdoc />
        public string PathToConfigurationFiles { get; }

        /// <summary>
        /// Creates new instance of provider of all configurations for application.
        /// </summary>
        /// <param name="pathToConfigurationFiles">Path to directory where configurations are located.
        /// If params is <see langoword="null"/> or empty, current working directory will be used.</param>
        /// <param name="cliArgs">Command line arguments.</param>
        /// <exception cref="ArgumentException">If directory does not exist</exception>
        protected ConfigurationProviderBase(string? pathToConfigurationFiles = null, bool isConfigOptional = false, string[]? cliArgs = null)
        {
            _isConfigOptional = isConfigOptional;
            _cliArgs = cliArgs;

            if (String.IsNullOrWhiteSpace(pathToConfigurationFiles))
            {
                PathToConfigurationFiles = Environment.CurrentDirectory;
            }
            else
            {
                if (!Directory.Exists(pathToConfigurationFiles))
                    throw new ArgumentException($"Directory \"{pathToConfigurationFiles}\" with configurations does not exist. Please, check app running options and directory existence.");

                PathToConfigurationFiles = pathToConfigurationFiles;
            }
        }

        /// <inheritdoc />
        public IConfiguration GetRawConfiguration()
        {
            if (_configuration != null) 
                return _configuration;

            var builder = new ConfigurationBuilder();
            ConfigureAppConfiguration(builder);
        
            _configuration = builder.Build();

            return _configuration;
        }
        
        /// <inheritdoc />
        public T GetConfiguration()
        {
            if (_typedConfiguration != null) 
                return _typedConfiguration;

            var rawConfiguration = GetRawConfiguration();

            _typedConfiguration = rawConfiguration.Get<T>();

            return _typedConfiguration;
        }

        public void ConfigureAppConfiguration(IConfigurationBuilder configuration)
        {
            if (configuration == null)
                throw new ArgumentNullException(nameof(configuration));

            // configure path
            SetBasePath(configuration, PathToConfigurationFiles);

            // add default config file
            var files = new List<(string fileName, bool isOptional)>(1)
            {
                (DefaultConfigurationFileName, _isConfigOptional)
            };

            // add optional environment files
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (!String.IsNullOrWhiteSpace(environment) &&
                !String.Equals(environment, ProductionEnvironmentName, StringComparison.OrdinalIgnoreCase))
            {
                files.Add((String.Format(EnvironmentConfigurationFileNameTemplate, environment), true));

                // if user was specified, add options environment specific config file for uer
                var user = Environment.GetEnvironmentVariable("USER");
                if (!String.IsNullOrWhiteSpace(user))
                {
                    files.Add((String.Format(EnvironmentAndUserConfigurationFileNameTemplate, environment, user), true));
                }
            }

            // add files
            foreach (var (fileName, isOptional) in files)
            {
                AddFile(configuration, fileName, isOptional);
            }

            // add environment variable
            configuration.AddEnvironmentVariables();
            
            // ddd CLI args
            if (_cliArgs != null)
            {
                configuration.AddCommandLine(_cliArgs);
            }
        }

        /// <summary>
        /// Sets base path to directory with configurations.
        /// </summary>
        protected abstract void SetBasePath(IConfigurationBuilder configurationBuilder, string basePath);
        
        /// <summary>
        /// Adds specified file to <paramref name="configurationBuilder"/>. 
        /// </summary>
        protected abstract void AddFile(IConfigurationBuilder configurationBuilder, string fileNameWithoutExtension, bool isFileOptional);
    }
}