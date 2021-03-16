using System.Threading.Tasks;

namespace Curiosity.Notification.Abstractions
{
    /// <summary>
    /// Chanel by which a notification will be sent
    /// </summary>
    public interface INotificationChannel
    {
        string ChannelType { get; }
        
        Task SendNotificationAsync(INotification notifications);
    }
}