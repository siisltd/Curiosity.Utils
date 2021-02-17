using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Curiosity.Tools
{
    public static class IoCExtensions
    {
        /// <summary>
        /// Adds to IoC date and time service that uses local machine time system.
        /// </summary>
        public static IServiceCollection AddLocalDateTimeService(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            
            services.TryAddSingleton<IDateTimeService, LocalDateTimeService>();

            return services;
        }
    }
}