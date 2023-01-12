using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Curiosity.Hosting.AppInitializer
{
    /// <summary>
    /// Provides extension methods to register async initializers.
    /// </summary>
    public static class IoCExtensions
    {
        /// <summary>
        /// Registers necessary services for async initialization support.
        /// </summary>
        /// <param name="services">The <see cref="T:Microsoft.Extensions.DependencyInjection.IServiceCollection" /> to add the service to.</param>
        /// <returns>A reference to this instance after the operation has completed.</returns>
        public static IServiceCollection AddAppInitialization(this IServiceCollection services)
        {
            if (services == null)
                throw new ArgumentNullException(nameof(services));

            services.TryAddTransient<AppInitializer>();
            return services;
        }
    }
}
