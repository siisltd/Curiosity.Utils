using System;
using System.Threading.Tasks;
using Curiosity.EMail.Smtp;
using Curiosity.Notification.Types;
using Microsoft.Extensions.Logging;

namespace Curiosity.Notification.Channels
{
    public class EmailChannel : NotificationChannelBase<EmailNotification>
    {
        public new string ChannelType => EmailNotification.Type;
        private readonly ISmtpEMailSender _sender;

        public EmailChannel(ILogger<EmailChannel> logger, ISmtpEMailSender sender) : base(logger, EmailNotification.Type)
        {
            _sender = sender;
        }
        
        protected override async Task ProcessNotificationAsync(EmailNotification notification)
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));
            
            if (String.IsNullOrWhiteSpace(notification.Email))
            {
                Logger.LogWarning("Incorrect email for notification");
                return;
            }

            var result = await _sender.SendAsync(
                notification.Email,
                notification.Subject,
                notification.Body,
                notification.IsBodyHtml);
            if (!result)
                throw new InvalidOperationException($"Sending email to {notification.Email} failed");
        }
    }
}