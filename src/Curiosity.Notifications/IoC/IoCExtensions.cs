using System;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace Curiosity.Notifications
{
    /// <summary>
    /// Extension methods for IoC for notification classes.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Add to IoC service for sending and processing notifications.
        /// </summary>
        public static IServiceCollection AddCuriosityNotificator(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.TryAddSingleton<INotificator, Notificator>();

            return services;
        }

        /// <summary>
        /// Adds specified notification channel.
        /// </summary>
        /// <remarks>
        /// Channel is added as hosted service and notification channel.
        /// </remarks>
        public static IServiceCollection AddCuriosityNotificationChannel<T>(this IServiceCollection services) where T: class, INotificationChannel, IHostedService
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddCuriosityNotificator();
            
            services.TryAddSingleton<T>();
            services.AddSingleton<INotificationChannel>(c => c.GetRequiredService<T>());
            services.AddSingleton<IHostedService>(c => c.GetRequiredService<T>());

            return services;
        }

        /// <summary>
        /// Adds specified notification channel.
        /// </summary>
        /// <remarks>
        /// Channel is added as hosted service and notification channel.
        /// </remarks>
        public static IServiceCollection AddCuriosityNotificationChannel<TType, TImplementation>(this IServiceCollection services) 
            where TType: class, INotificationChannel
            where TImplementation: class, TType, INotificationChannel, IHostedService
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddCuriosityNotificator();
            
            services.TryAddSingleton<TImplementation>();
            services.AddSingleton<TType>(c => c.GetRequiredService<TImplementation>());
            services.AddSingleton<INotificationChannel>(c => c.GetRequiredService<TImplementation>());
            services.AddSingleton<IHostedService>(c => c.GetRequiredService<TImplementation>());

            return services;
        }

        /// <summary>
        /// Adds specified notification builder to IoC.
        /// </summary>
        public static IServiceCollection AddCuriosityNotificationBuilder<T>(this IServiceCollection services) where T : class, INotificationBuilder
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddSingleton<INotificationBuilder, T>();

            return services;
        }
    }
}
