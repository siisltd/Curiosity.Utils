using System;
using Curiosity.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Curiosity.SMS.Smsc
{
    public static class IoCExtensions
    {
        /// <summary>
        /// Adds SMTP EMail sender to services.
        /// </summary>
        public static IServiceCollection AddSmscSmsSender(
            this IServiceCollection services,
            SmscOptions options,
            bool useAsDefaultSender = true)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (options == null) throw new ArgumentNullException(nameof(options));

            options.AssertValid();

            services.TryAddSingleton(options);
            services.TryAddSingleton<ISmscSender, SmscSender>();

            if (useAsDefaultSender)
            {
                services.TryAddSingleton<ISmsSender>(s => s.GetRequiredService<ISmscSender>());
            }

            return services;
        }
    }
}
