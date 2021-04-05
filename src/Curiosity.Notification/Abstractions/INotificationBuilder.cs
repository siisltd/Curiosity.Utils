using System.Collections.Generic;
using System.Threading.Tasks;

namespace Curiosity.Notification.Abstractions
{
    /// <summary>
    /// Builder which created notifications from metadata
    /// </summary>
    public interface INotificationBuilder
    {
        string NotificationType { get; }
        
        string ChannelType { get; }

        Task<IReadOnlyList<INotification>> BuildNotificationsAsync(INotificationMetadata notificationMetadata);
    }
}