using System;
using Curiosity.Notification.Abstractions;
using Curiosity.Notification.Channels;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Curiosity.Notification.IoC
{
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds default email channel as hosted service and notification channel
        /// </summary>
        private static IServiceCollection AddEmailChannel(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            
            services.TryAddSingleton<INotificator, Notificator>();
            
            services.AddSingleton<EmailChannel>();
            services.AddSingleton<INotificationChannel>(c => c.GetRequiredService<EmailChannel>());
            services.AddSingleton<IHostedService>(c => c.GetRequiredService<EmailChannel>());

            return services;
        }
    }
}