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
        private const string SecretFileNameTemplate = "{0}.secret";
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
            var files = new List<(string fileName, bool isOptional)>(2)
            {
                (DefaultConfigurationFileName, _isConfigOptional),
                (String.Format(SecretFileNameTemplate, DefaultConfigurationFileName), true)
            };

            // add optional environment files
            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (!String.IsNullOrWhiteSpace(environment) &&
                !String.Equals(environment, ProductionEnvironmentName, StringComparison.OrdinalIgnoreCase))
            {
                var envConfigFileName = String.Format(EnvironmentConfigurationFileNameTemplate, environment);
                files.Add((envConfigFileName, true));
                files.Add((String.Format(SecretFileNameTemplate, envConfigFileName), true));

                // if user was specified, add options environment specific config file for uer
                var user = Environment.GetEnvironmentVariable("USER");
                if (!String.IsNullOrWhiteSpace(user))
                {
                    var userEnvConfigFileName = String.Format(EnvironmentAndUserConfigurationFileNameTemplate, environment, user);
                    files.Add((userEnvConfigFileName, true));
                    files.Add((String.Format(SecretFileNameTemplate, userEnvConfigFileName), true));
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