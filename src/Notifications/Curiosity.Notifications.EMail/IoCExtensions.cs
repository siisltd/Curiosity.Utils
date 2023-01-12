using System;
using Curiosity.EMail;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

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

            services.AddCuriosityNotificationChannel<IEmailNotificationChannel, EmailNotificationChannel>();

            return services;
        }

        /// <summary>
        /// Adds to IoC specified EMail notification post processor.
        /// </summary>
        /// <typeparam name="T">Type of post processor.</typeparam>
        public static IServiceCollection AddEMailNotificationPostProcessor<T>(this IServiceCollection services) where T: class, IEMailNotificationPostProcessor
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.TryAddSingleton<IEMailNotificationPostProcessor, T>();

            return services;
        }
    }
}
