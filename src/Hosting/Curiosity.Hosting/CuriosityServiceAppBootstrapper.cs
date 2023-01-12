using System;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.Configuration;
using Curiosity.Hosting.AppInitializer;
using Curiosity.Hosting.Performance;
using Curiosity.Tools.AppInitializer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace Curiosity.Hosting
{
    /// <summary>
    /// Bootstrapper for service workers.
    /// </summary>
    public abstract class CuriosityServiceAppBootstrapper<TArgs, TConfiguration>: CuriosityAppBootstrapper<TArgs, TConfiguration> 
        where TArgs : CuriosityCLIArguments, new() 
        where TConfiguration : class, ICuriosityAppConfiguration, new()
    {
        private Action<HostBuilderContext, IServiceCollection>? _configureServiceAction;
        private Action<HostBuilderContext, IServiceCollection, TConfiguration>? _configureServiceActionWithConfiguration;
        private Action<HostBuilderContext, IServiceCollection, TConfiguration, TArgs>? _configureServiceActionWithConfigurationAndArgs;
        private Action<IHostBuilder>? _configureHostAction;
        private Action<IHostBuilder, TConfiguration>? _configureHostActionWithConfiguration;
        private Action<IHostBuilder, TArgs>? _configureHostActionWithCLIArgis;
        private Action<IHostBuilder, TConfiguration, TArgs>? _configureHostActionWithConfigurationAndCLIArgis;

        /// <summary>
        /// Configures services using specified delegate.
        /// </summary>
        /// <param name="configureServiceAction">Delegate for configuration services.</param>
        /// <returns>Returns current bootstrapper instance.</returns>
        public CuriosityServiceAppBootstrapper<TArgs, TConfiguration> ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureServiceAction)
        {
            _configureServiceAction = configureServiceAction;
            return this;
        }

        /// <summary>
        /// Configures services using specified delegate.
        /// </summary>
        /// <param name="configureServiceAction">Delegate for configuration services using configuration object.</param>
        /// <returns>Returns current bootstrapper instance.</returns>
        public CuriosityServiceAppBootstrapper<TArgs, TConfiguration> ConfigureServices(Action<HostBuilderContext, IServiceCollection, TConfiguration> configureServiceAction)
        {
            _configureServiceActionWithConfiguration = configureServiceAction;
            return this;
        }

        /// <summary>
        /// Configures services using specified delegate.
        /// </summary>
        /// <param name="configureServiceAction">Delegate for configuration services using configuration and args objects.</param>
        /// <returns>Returns current bootstrapper instance.</returns>
        public CuriosityServiceAppBootstrapper<TArgs, TConfiguration> ConfigureServices(
            Action<HostBuilderContext, IServiceCollection, TConfiguration, TArgs> configureServiceAction)
        {
            _configureServiceActionWithConfigurationAndArgs = configureServiceAction;
            return this;
        }

        /// <summary>
        /// Configures app's host using specified delegate.
        /// </summary>
        /// <param name="configureHostAction">Delegate for configuration host via <see cref="IHostBuilder"/>.</param>
        /// <returns>Returns current bootstrapper instance.</returns>
        public CuriosityServiceAppBootstrapper<TArgs, TConfiguration> ConfigureHost(Action<IHostBuilder> configureHostAction)
        {
            _configureHostAction = configureHostAction;
            return this;
        }

        /// <summary>
        /// Configures app's host using specified delegate.
        /// </summary>
        /// <param name="configureHostAction">Delegate for configuration host via <see cref="IHostBuilder"/>.</param>
        /// <returns>Returns current bootstrapper instance.</returns>
        public CuriosityServiceAppBootstrapper<TArgs, TConfiguration> ConfigureHost(Action<IHostBuilder, TConfiguration> configureHostAction)
        {
            _configureHostActionWithConfiguration = configureHostAction;
            return this;
        }

        /// <summary>
        /// Configures app's host using specified delegate.
        /// </summary>
        /// <param name="configureHostAction">Delegate for configuration host via <see cref="IHostBuilder"/>.</param>
        /// <returns>Returns current bootstrapper instance.</returns>
        public CuriosityServiceAppBootstrapper<TArgs, TConfiguration> ConfigureHost(Action<IHostBuilder, TArgs> configureHostAction)
        {
            _configureHostActionWithCLIArgis = configureHostAction;
            return this;
        }

        /// <summary>
        /// Configures app's host using specified delegate.
        /// </summary>
        /// <param name="configureHostAction">Delegate for configuration host via <see cref="IHostBuilder"/>.</param>
        /// <returns>Returns current bootstrapper instance.</returns>
        public CuriosityServiceAppBootstrapper<TArgs, TConfiguration> ConfigureHost(Action<IHostBuilder, TConfiguration, TArgs> configureHostAction)
        {
            _configureHostActionWithConfigurationAndCLIArgis = configureHostAction;
            return this;
        }

        /// <inheritdoc />
        protected override async Task<int> RunInternalAsync(
            string[] rawArguments,
            TArgs arguments,
            TConfiguration configuration,
            IConfigurationProvider<TConfiguration> configurationProvider,
            string customContentRootDirectory,
            CancellationToken cancellationToken = default)
        {
            var hostBuilder = Host.CreateDefaultBuilder(rawArguments);
            
            // configure configuration
            hostBuilder.ConfigureAppConfiguration((context, configurationBuilder) =>
            {
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                context.HostingEnvironment.EnvironmentName = environment;
                configurationProvider.ConfigureAppConfiguration(configurationBuilder);
            });
            
            // configure services
            hostBuilder.ConfigureServices((context, services) =>
            {
                services.AddSingleton<ILogger>(c =>
                {
                    var loggerFactory = c.GetRequiredService<ILoggerFactory>();
                    return loggerFactory.CreateLogger("main");
                });
                services.AddLocalization(opt =>
                {
                    opt.ResourcesPath = "Resources";
                });
                _configureServiceAction?.Invoke(context, services);
                _configureServiceActionWithConfiguration?.Invoke(context, services, configuration);
                _configureServiceActionWithConfigurationAndArgs?.Invoke(context, services, configuration, arguments);
                services.AddAppInitialization();
                
                services.TryAddSingleton(configuration);
                services.TryAddSingleton(arguments);
                services.TryAddSingleton(configurationProvider);

                services.AddAppInitializer<FireAndForgetInitializer>();
                services.AddPerformanceMeasures();
            });
            
            // configure logging
            hostBuilder.ConfigureLogging((context, logging) =>
            {
                logging.ClearProviders();
                logging.SetMinimumLevel(LogLevel.Trace);
                logging.AddNLog();
            });
            
            // configure host
            _configureHostAction?.Invoke(hostBuilder);
            _configureHostActionWithConfiguration?.Invoke(hostBuilder, configuration);
            _configureHostActionWithCLIArgis?.Invoke(hostBuilder, arguments);
            _configureHostActionWithConfigurationAndCLIArgis?.Invoke(hostBuilder, configuration, arguments);

            // init and run
            var host = hostBuilder.Build();
            
            await host.InitAsync(cancellationToken);
            await host.RunAsync(cancellationToken);

            return CuriosityExitCodes.Success;
        }
    }
}
