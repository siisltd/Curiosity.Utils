using System;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.EMail;
using Microsoft.Extensions.Logging;

namespace Curiosity.Notifications.EMail
{
    //todo Add notification post processor

    /// <summary>
    /// Channel for sending EMail notifications.
    /// Uses registered at IoC implementation of <see cref="IEMailSender"/> to send notifications.
    /// </summary>
    public class EmailNotificationChannel : NotificationChannelBase<EmailNotification>
    {
        private readonly IEMailSender _sender;

        public EmailNotificationChannel(ILogger<EmailNotificationChannel> logger, IEMailSender sender) : base(logger, EmailNotification.Type)
        {
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));
        }

        /// <inheritdoc />
        protected override async Task ProcessNotificationAsync(EmailNotification notification, CancellationToken cancellationToken = default)
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));

            var result = notification.ExtraParams == null
                ? await _sender.SendAsync(
                    notification.Email,
                    notification.Subject,
                    notification.Body,
                    notification.IsBodyHtml,
                    cancellationToken)
                : await _sender.SendAsync(
                    notification.Email,
                    notification.Subject,
                    notification.Body,
                    notification.IsBodyHtml,
                    notification.ExtraParams,
                    cancellationToken);
            if (!result)
                throw new InvalidOperationException($"Sending email to \"{notification.Email}\" failed");
        }
    }
}
