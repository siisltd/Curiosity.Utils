using System;
using Curiosity.SMS;
using Microsoft.Extensions.DependencyInjection;

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
    }
}
