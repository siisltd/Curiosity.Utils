using System;
using Curiosity.AppInitializer;
using Curiosity.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Curiosity.Hosting.ThreadPool
{
    /// <summary>
    /// Extension method for <see cref="IServiceCollection"/> to add thread pool tuning.
    /// </summary>
    public static class IoCExtensions
    {
        /// <summary>
        /// Adds to services thread pool monitoring and tuning classes.
        /// </summary>
        public static IServiceCollection AddThreadPoolTuning(
            this IServiceCollection services,
            ThreadPoolOptions threadPoolOptions)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (threadPoolOptions == null) throw new ArgumentNullException(nameof(threadPoolOptions));
            
            threadPoolOptions.AssertValid();

            services.TryAddSingleton(threadPoolOptions);
            services.AddAppInitializer<ThreadPoolInitializer>();
            services.AddSingleton<IHostedService, ThreadPoolMonitoringService>();
            
            return services;
        }
    }
}