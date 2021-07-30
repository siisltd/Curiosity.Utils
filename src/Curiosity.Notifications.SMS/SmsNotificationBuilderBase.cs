using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Curiosity.Notifications.SMS
{
    /// <summary>
    /// Base class for building SMS notifications from metadata.
    /// </summary>
    /// <typeparam name="TMetadata">Type of a notifications metadata.</typeparam>
    public abstract class SmsNotificationBuilderBase<TMetadata> : INotificationBuilder where TMetadata : INotificationMetadata
    {
        /// <inheritdoc />
        public abstract string NotificationType { get; }

        /// <inheritdoc />
        public string ChannelType { get; } = SmsNotification.Type;

        /// <inheritdoc />
        public Task<IReadOnlyList<INotification>> BuildNotificationsAsync(INotificationMetadata notificationMetadata, CancellationToken cancellationToken = default)
        {
            if (notificationMetadata == null) throw new ArgumentNullException(nameof(notificationMetadata));
            if (!(notificationMetadata is TMetadata metadata))
                throw new ArgumentException($"{GetType().Name} supports only {typeof(TMetadata).Name}");

            return BuildNotificationsAsync(metadata, cancellationToken);
        }

        /// <summary>
        /// Builds a notification from specified metadata.
        /// </summary>
        protected abstract Task<IReadOnlyList<INotification>> BuildNotificationsAsync(TMetadata metadata, CancellationToken cancellationToken = default);
    }
}
