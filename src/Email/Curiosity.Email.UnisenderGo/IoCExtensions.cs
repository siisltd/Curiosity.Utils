using System;
using Curiosity.Configuration;
using Curiosity.EMail;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Curiosity.Email.UnisenderGo
{
    /// <summary>
    /// Extension methods for registering UnisenderGo email services in <see cref="IServiceCollection"/>.
    /// </summary>
    public static class IoCExtensions
    {
        /// <summary>
        /// Adds UnisenderGo email sender to services.
        /// </summary>
        public static IServiceCollection AddUnisenderGoEmailSender(
            this IServiceCollection services,
            UnisenderGoEmailOptions options,
            bool useAsDefaultSender = true)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (options == null) throw new ArgumentNullException(nameof(options));

            options.AssertValid();
            
            services.TryAddSingleton(options);
            services.TryAddSingleton<IUnisenderGoEmailSender, UnisenderGoEmailSender>();

            if (useAsDefaultSender)
            {
                services.TryAddSingleton<IEMailSender>(s => s.GetRequiredService<IUnisenderGoEmailSender>());
            }

            return services;
        }
    }
}
