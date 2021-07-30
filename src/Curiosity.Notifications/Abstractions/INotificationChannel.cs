using System.Threading;
using System.Threading.Tasks;

namespace Curiosity.Notifications
{
    /// <summary>
    /// The channel to which the notification is sent.
    /// </summary>
    public interface INotificationChannel
    {
        /// <summary>
        /// Type of the channel.
        /// </summary>
        string ChannelType { get; }

        /// <summary>
        /// Sends notification via this channel.
        /// </summary>
        /// <param name="notifications">Notification to send.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        Task SendNotificationAsync(INotification notifications, CancellationToken cancellationToken = default);
    }
}
