using Curiosity.AppInitializer;
using Microsoft.Extensions.DependencyInjection;

namespace Curiosity.Hosting.Performance
{
    public static class IoCExtensions
    {
        /// <summary>
        /// Adds to IoC classes for measuring performance and stuck code. 
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddPerformanceMeasures(this IServiceCollection services)
        {
            services.AddAppInitializer<PerformanceMonitoringInitializer>();
            return services;
        }
    }
}