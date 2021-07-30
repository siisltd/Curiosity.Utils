using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Curiosity.Notifications
{
    /// <summary>
    /// Builder which creates notifications from metadata
    /// </summary>
    public interface INotificationBuilder
    {
        /// <summary>
        /// Type of the notification that will be created.
        /// </summary>
        string NotificationType { get; }
        
        /// <summary>
        /// Type of the channel for which the notification is being created.
        /// </summary>
        string ChannelType { get; }

        /// <summary>
        /// Builds a notification from specified metadata.
        /// </summary>
        Task<IReadOnlyList<INotification>> BuildNotificationsAsync(INotificationMetadata notificationMetadata, CancellationToken cancellationToken = default);
    }
}
