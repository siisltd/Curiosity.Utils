using System;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.AppInitializer;
using Curiosity.Configuration;
using Curiosity.Hosting.Performance;
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
        private Action<IHostBuilder>? _configureHostAction;
        
        public CuriosityServiceAppBootstrapper<TArgs, TConfiguration> ConfigureServices(Action<HostBuilderContext, IServiceCollection> configureServiceAction)
        {
            _configureServiceAction = configureServiceAction;
            return this;
        }
        public CuriosityServiceAppBootstrapper<TArgs, TConfiguration> ConfigureServices(Action<HostBuilderContext, IServiceCollection, TConfiguration> configureServiceAction)
        {
            _configureServiceActionWithConfiguration = configureServiceAction;
            return this;
        }

        public CuriosityServiceAppBootstrapper<TArgs, TConfiguration> ConfigureHost(Action<IHostBuilder> configureHostAction)
        {
            _configureHostAction = configureHostAction;
            return this;
        }
        
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
            hostBuilder.ConfigureAppConfiguration((context, _configuration) =>
            {
                var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                context.HostingEnvironment.EnvironmentName = environment;
                configurationProvider.ConfigureAppConfiguration(_configuration);
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

            // init and run
            var host = hostBuilder.Build();
            
            await host.InitAsync(cancellationToken);
            await host.RunAsync(cancellationToken);

            return CuriosityExitCodes.Success;
        }
    }
}