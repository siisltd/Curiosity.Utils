using System;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.AppInitializer;
using Curiosity.Configuration;
using Curiosity.Hosting.ThreadPool;
using Curiosity.Tools.Web;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Web;

namespace Curiosity.Hosting.Web
{
    /// <summary>
    /// Bootstrapper for web app.
    /// </summary>
    /// <typeparam name="TArgs">CLI arguments type.</typeparam>
    /// <typeparam name="TConfiguration">POCO configuration type.</typeparam>
    /// <typeparam name="TStartup">Startup class.</typeparam>
    public class CuriosityWebAppBootstrapper<TArgs, TConfiguration, TStartup> : CuriosityAppBootstrapper<TArgs, TConfiguration>
        where TArgs : CuriosityCLIArguments, new()
        where TConfiguration : class, ICuriosityWebAppConfiguration, new()
        where TStartup : class
    {
        /// <inheritdoc />
        protected override async Task<int> RunInternalAsync(
            string[] rawArguments,
            TArgs arguments,
            TConfiguration configuration,
            IConfigurationProvider<TConfiguration> configurationProvider,
            string customContentRootDirectory,
            CancellationToken cancellationToken = default)
        {
            var host = CreateWebHost(
                rawArguments,
                arguments,
                configuration,
                configurationProvider,
                customContentRootDirectory);
            await host.InitAsync(cancellationToken);
            await host.RunAsync(cancellationToken);

            return CuriosityExitCodes.Success;
        }

        private static IHost CreateWebHost(
            string[] args,
            TArgs arguments,
            TConfiguration configuration,
            IConfigurationProvider<TConfiguration> configurationProvider,
            string? customContentRootDirectory)
        {
            var hostBuilder = Host.CreateDefaultBuilder(args);
            
            hostBuilder
                .ConfigureServices(services =>
                {
                    services.AddSingleton<ILogger>(c =>
                    {
                        var loggerFactory = c.GetRequiredService<ILoggerFactory>();
                        return loggerFactory.CreateLogger("main");
                    });
                    
                    services.TryAddSingleton(configuration);
                    services.TryAddSingleton(arguments);
                    
                    // ReSharper disable once SuspiciousTypeConversion.Global
                    if (configuration is IWebAppConfigurationWithPublicDomain publicDomainConfig)
                    {
                        services.TryAddSingleton(publicDomainConfig);
                    }
                    
                    services.TryAddSingleton(configurationProvider);
                    services.AddAppInitialization();

                    services.AddThreadPoolTuning(configuration.ThreadPool);
                    services.AddAppInitializer<FireAndForgetInitializer>();
                })
                .ConfigureAppConfiguration((context, configurationBuilder) =>
                {
                    var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
                    context.HostingEnvironment.EnvironmentName = environment;
                    configurationProvider.ConfigureAppConfiguration(configurationBuilder);
                })
                .ConfigureLogging(logging =>
                {
                    logging.ClearProviders();
                    logging.SetMinimumLevel(LogLevel.Trace);
                });
            
            hostBuilder.ConfigureWebHost(webHostBuilder =>
            {
                // use specified kestrel options
                if (configuration.Kestrel != null)
                {
                    webHostBuilder
                        .UseKestrel((context, severOptions) =>
                        {
                            // pass "raw" configuration section instead of pass POCO, because we don't need all options available from code
                            // Kestrel will find all necessary options
                            severOptions.Configure(configurationProvider.GetRawConfiguration().GetSection("Kestrel"));
                        });
                }
                else // or just basic kestrel options with specified urls
                {
                    webHostBuilder
                        .UseKestrel()
                        .UseUrls(configuration.Urls);
                }

                webHostBuilder
                    .UseNLog()
                    .UseStartup<TStartup>();
            });

            if (!String.IsNullOrWhiteSpace(customContentRootDirectory))
                hostBuilder.UseContentRoot(customContentRootDirectory);

            // configures logging
            // NLogBuilder.ConfigureNLog(configuration.Log.LogConfigurationPath);

            return hostBuilder.Build();
        }

        protected override void LoadLoggingConfiguration(string path)
        {
            NLogBuilder.ConfigureNLog(path);
        }
    }
}