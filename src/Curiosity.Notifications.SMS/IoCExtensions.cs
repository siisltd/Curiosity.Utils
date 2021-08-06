using System;
using Curiosity.SMS;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Curiosity.Notifications.SMS
{
    public static class IoCExtensions
    {
        /// <summary>
        /// Adds to IoC SMS channel that uses implementation of <see cref="ISmsSender"/> to send SMS notifications.
        /// </summary>
        public static IServiceCollection AddCuriositySmsChannel(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddCuriosityNotificationChannel<SmsNotificationChannel>();

            return services;
        }

        /// <summary>
        /// Adds to IoC specified SMS notification post processor.
        /// </summary>
        /// <typeparam name="T">Type of post processor.</typeparam>
        public static IServiceCollection AddSmsNotificationPostProcessor<T>(this IServiceCollection services) where T: class, ISmsNotificationPostProcessor
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.TryAddSingleton<ISmsNotificationPostProcessor, T>();

            return services;
        }
    }
}
