using System;
using System.Threading;
using System.Threading.Tasks;

namespace Curiosity.Notifications
{
    /// <summary>
    /// Service for sending notifications.
    /// </summary>
    /// <remarks>
    /// Create a notification from metadata, chose notification channel
    /// and post notification to this channel. 
    /// </remarks>
    public interface INotificator
    {
        /// <summary>
        /// Sends notification in async manners, wait for sending notification to each available channel. 
        /// </summary>
        /// <param name="notificationMetadata">Notification's metadata.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Is arguments are incorrect.</exception>
        /// <exception cref="InvalidOperationException">If no channels available.</exception>
        Task NotifyAsync(INotificationMetadata notificationMetadata, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends notification without waiting for sending completion.
        /// </summary>
        /// <param name="notificationMetadata">Notification's metadata.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">Is arguments are incorrect.</exception>
        void NotifyAndForgot(INotificationMetadata notificationMetadata);
    }
}
