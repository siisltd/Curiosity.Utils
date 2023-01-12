using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Curiosity.EMail
{
    public static class IoCExtensions
    {
        /// <summary>
        /// Adds <see cref="TestLogEmailSender"/> to services.
        /// </summary>
        public static IServiceCollection AddTestLogEMailSender(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.TryAddSingleton<IEMailSender, TestLogEmailSender>();
            return services;
        }
    }
}
