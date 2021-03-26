using System;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EntryPoint;
using Curiosity.Configuration;
using Curiosity.Configurations;
using Curiosity.Tools;
using NLog;

namespace Curiosity.Hosting
{
    /// <summary>
    /// Bootstrapper for basic Marvin's app.
    /// </summary>
    /// <typeparam name="TArgs">CLI arguments type.</typeparam>
    /// <typeparam name="TConfiguration">POCO configuration type.</typeparam>
    public abstract class CuriosityAppBootstrapper<TArgs, TConfiguration>
        where TArgs : CuriosityCLIArguments, new()
        where TConfiguration : class, ICuriosityAppConfiguration, new()
    {
        protected bool IsConfigFileOptional { get; set; } = false;
        
        /// <summary>
        /// Runs application.
        /// </summary>
        /// <param name="args">CLI arguments.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        public async Task<int> RunAsync(string[] args, CancellationToken cancellationToken = default)
        {
            await Task.Yield();
            
            // process CLI args
            var arguments = Cli.Parse<TArgs>(args);
            if (arguments.HelpInvoked) return 0;
            if (arguments.ShowVersion)
            {
                Console.WriteLine($"App version: {ApplicationHelper.GetAssemblyVersion()}");
                return CuriosityExitCodes.Success;
            }

            // basic app setup: culture, content root directory
            var customContentRootDirectory = String.Empty;

            if (EnvironmentHelper.IsDevelopmentEnvironment())
            {
                customContentRootDirectory = Directory.GetCurrentDirectory();
            }

            ApplicationHelper.ChangeCurrentDirectoryForDevelopment();

            // get configuration
            var configurationProvider = new YamlConfigurationProvider<TConfiguration>(arguments.ConfigurationDirectory, IsConfigFileOptional, args);
            var configuration = configurationProvider.GetConfiguration();

            ApplicationHelper.SetDefaultCulture(configuration.Culture);

            // configure logs
            var loggingConfiguration = new MarvinNLogConfigurator(configuration.Log.LogConfigurationPath, LoadLoggingConfiguration);
            loggingConfiguration.WithLogOutputDirectory(configuration.Log.LogOutputDirectory);
            
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (configuration is IConfigurationWithMailLogger configurationWithMailLogger)
            {
                loggingConfiguration.WithMail(configuration.AppName, configurationWithMailLogger.LoggerMail);
            }
            loggingConfiguration.Configure();

            var logger = LogManager.GetCurrentClassLogger();
            logger.Info($"Using configurations from directory \"{configurationProvider.PathToConfigurationFiles}\"'");

            // print configuration
            var configPrinter = new ConfigurationPrinter();
            var configLog = configPrinter.GetLog(configuration);
            logger.Info($"Starting app with configuration: \n{configLog ?? "<no config>"}");
            
            // validate configuration
            var errors = configuration.Validate();
            if (errors.Count == 0)
            {
                logger.Info("Configuration is valid.");
            }
            else
            {
                var errorsBuilder = new StringBuilder();
                foreach (var validationError in errors)
                {
                    errorsBuilder.AppendLine($"- {validationError.FieldName}: {validationError.Error}");
                }

                logger.Error($"Configuration is invalid. Errors: {Environment.NewLine}{errorsBuilder}");

                return CuriosityExitCodes.IncorrectConfiguration;
            }

            try
            {
                var startEmailLogger = LogManager.GetLogger("appStartEmailLog");
                startEmailLogger.Info($"{configuration.AppName} is started");

                return await RunInternalAsync(args, arguments, configuration, configurationProvider, customContentRootDirectory, cancellationToken);
            }
            catch (Exception e) when (e is OperationCanceledException || e is TaskCanceledException)
            {
                logger.Warn(e, $"{configuration.AppName} was cancelled.");

                return CuriosityExitCodes.Cancellation;
            }
            catch (Exception e)
            {
                logger.Fatal(e, $"Critical error on {configuration.AppName} work");

                return CuriosityExitCodes.UnhandledException;
            }
            finally
            {
                var startEmailLogger = LogManager.GetLogger("appStopEmailLog");
                startEmailLogger.Info($"{configuration.AppName} is stopped");
                // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
                LogManager.Shutdown();
            }
        }
        
        protected virtual void LoadLoggingConfiguration(string path)
        {
            LogManager.LoadConfiguration(path);
        }
        
        /// <summary>
        /// Runs application.
        /// </summary>
        protected abstract Task<int> RunInternalAsync(
            string[] rawArguments,
            TArgs arguments,
            TConfiguration configuration,
            IConfigurationProvider<TConfiguration> configurationProvider,
            string customContentRootDirectory,
            CancellationToken cancellationToken = default);
    }
}