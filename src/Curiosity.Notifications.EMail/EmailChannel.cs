using System;
using System.Threading.Tasks;
using Curiosity.EMail;
using Microsoft.Extensions.Logging;

namespace Curiosity.Notifications.EMail
{
    /// <summary>
    /// Channel for sending EMail notifications.
    /// Uses registered at IoC implementation of <see cref="IEMailSender"/> to send notifications.
    /// </summary>
    public class EmailChannel : NotificationChannelBase<EmailNotification>
    {
        private readonly IEMailSender _sender;

        public EmailChannel(ILogger<EmailChannel> logger, IEMailSender sender) : base(logger, EmailNotification.Type)
        {
            _sender = sender;
        }

        /// <inheritdoc />
        protected override async Task ProcessNotificationAsync(EmailNotification notification)
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));
            
            if (String.IsNullOrWhiteSpace(notification.Email))
            {
                Logger.LogWarning("Empty email for notification. Notification will not be send.");
                return;
            }

            var result = await _sender.SendAsync(
                notification.Email,
                notification.Subject,
                notification.Body,
                notification.IsBodyHtml);
            if (!result)
                throw new InvalidOperationException($"Sending email to \"{notification.Email}\" failed");
        }
    }
}