using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Curiosity.Notification.Abstractions;
using Curiosity.Notification.Types;

namespace Curiosity.Notification.Builders
{
    public abstract class BaseSmsNotificationBuilder<TMetadata> : INotificationBuilder where TMetadata : INotificationMetadata
    {
        public abstract string NotificationType { get; }
        public string ChannelType { get; } = SmsNotification.Type;
        
        public Task<IReadOnlyList<INotification>> BuildNotificationsAsync(INotificationMetadata notificationMetadata)
        {
            if (notificationMetadata == null) throw new ArgumentNullException(nameof(notificationMetadata));
            if (!(notificationMetadata is TMetadata metadata))
                throw new ArgumentException($"{GetType().Name} supports only {typeof(TMetadata).Name}");

            return BuildNotificationsAsync(metadata);
        }

        protected abstract Task<IReadOnlyList<INotification>> BuildNotificationsAsync(TMetadata metadata);
    }
}