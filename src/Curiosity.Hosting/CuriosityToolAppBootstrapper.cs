using System;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.AppInitializer;
using Curiosity.Configuration;
using Curiosity.Hosting.Performance;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace Curiosity.Hosting
{
    /// <summary>
    /// Bootstrapper for simple apps that requires DI.
    /// </summary>
    public abstract class CuriosityToolAppBootstrapper : CuriosityToolAppBootstrapper<CuriosityCLIArguments, CuriosityAppConfiguration>
    {
    }
    
    /// <summary>
    /// Bootstrapper for simple apps that requires DI.
    /// </summary>
    public abstract class CuriosityToolAppBootstrapper<TArgs> : CuriosityToolAppBootstrapper<TArgs, CuriosityAppConfiguration>
        where TArgs : CuriosityCLIArguments, new()
    {
    }

    /// <summary>
    /// Bootstrapper for simple apps that requires DI.
    /// </summary>
    public abstract class CuriosityToolAppBootstrapper<TArgs, TConfiguration>: CuriosityAppBootstrapper<TArgs, TConfiguration> 
        where TArgs : CuriosityCLIArguments, new() 
        where TConfiguration : class, ICuriosityAppConfiguration, new()
    {
        private Action<IServiceCollection>? _configureServiceAction;
        private Action<IServiceCollection, TConfiguration>? _configureServiceActionWithConfig;
        
        public CuriosityToolAppBootstrapper<TArgs, TConfiguration> ConfigureServices(Action<IServiceCollection> configureServiceAction)
        {
            _configureServiceAction = configureServiceAction;
            return this;
        }

        public CuriosityToolAppBootstrapper<TArgs, TConfiguration> ConfigureServices(Action<IServiceCollection, TConfiguration> configureServiceAction)
        {
            _configureServiceActionWithConfig = configureServiceAction;
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
            // configure service
            var services = new ServiceCollection();
            services.AddSingleton<ILogger>(c =>
            {
                var loggerFactory = c.GetRequiredService<ILoggerFactory>();
                return loggerFactory.CreateLogger("main");
            });
            services.AddLocalization(opt =>
            {
                opt.ResourcesPath = "Resources";
            });
            _configureServiceAction?.Invoke(services);
            _configureServiceActionWithConfig?.Invoke(services, configuration);
            services.AddAppInitialization();
                
            services.TryAddSingleton(configuration);
            services.TryAddSingleton(arguments);
            services.TryAddSingleton(configurationProvider);
            services.AddPerformanceMeasures();

            services.AddAppInitializer<FireAndForgetInitializer>();
            services.AddLogging(opt =>
            {
                opt.ClearProviders();
                opt.SetMinimumLevel(LogLevel.Trace);
                opt.AddNLog();
            });

            var sp = services.BuildServiceProvider();
            using (var scope = sp.CreateScope())
            {
                await scope.InitAsync(cancellationToken);

                return await ExecuteAsync(sp, rawArguments, arguments, configuration, cancellationToken);
            }
        }

        /// <summary>
        /// Executes specified tool's actions.
        /// </summary>
        protected abstract Task<int> ExecuteAsync(
            IServiceProvider serviceProvider, 
            string[] rawArguments,
            TArgs arguments,
            TConfiguration configuration,
            CancellationToken cancellationToken = default);
    }
}