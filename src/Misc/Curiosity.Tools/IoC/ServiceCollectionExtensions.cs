using System;
using Microsoft.Extensions.DependencyInjection;

namespace Curiosity.Tools.IoC
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds SensitiveDataProtector as singleton
        /// </summary>
        /// <param name="services"></param>
        /// <param name="fieldNames">Name collection of the fields the data in which we want to hide</param>
        public static IServiceCollection AddSensitiveDataProtector(this IServiceCollection services, string[]? fieldNames)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton(new SensitiveDataProtector(fieldNames ?? Array.Empty<string>()));
            return services;
        }
    }
}