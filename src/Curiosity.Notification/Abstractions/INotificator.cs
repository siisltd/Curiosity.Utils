using System.Threading.Tasks;

namespace Curiosity.Notification.Abstractions
{
    /// <summary>
    /// Service which send notifications
    /// </summary>
    public interface INotificator
    {
        Task NotifyAsync(INotificationMetadata notificationMetadata);

        void NotifyAndForgot(INotificationMetadata notificationMetadata);
    }
}