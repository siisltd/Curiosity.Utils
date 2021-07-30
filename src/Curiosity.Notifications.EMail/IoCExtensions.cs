using System;
using Curiosity.EMail;
using Microsoft.Extensions.DependencyInjection;

namespace Curiosity.Notifications.EMail
{
    /// <summary>
    /// Extensions methods for registering EMail channel at IoC.
    /// </summary>
    public static class IoCExtensions
    {
        /// <summary>
        /// Adds to IoC EMail channel that uses implementation of <see cref="IEMailSender"/> to send EMail notifications.
        /// </summary>
        public static IServiceCollection AddCuriosityEMailChannel(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddCuriosityNotificationChannel<EmailNotificationChannel>();

            return services;
        }
    }
}
