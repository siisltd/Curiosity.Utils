using System;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.SMS;
using Microsoft.Extensions.Logging;

namespace Curiosity.Notifications.SMS
{
    public class SmsNotificationChannel : NotificationChannelBase<SmsNotification>
    {
        private readonly ISmsSender _smsSender;

        public SmsNotificationChannel(ILogger logger, ISmsSender smsSender) : base(logger, SmsNotification.Type)
        {
            _smsSender = smsSender ?? throw new ArgumentNullException(nameof(smsSender));
        }

        protected override async Task ProcessNotificationAsync(SmsNotification notification, CancellationToken cancellationToken = default)
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));

            var result = notification.ExtraParams == null
                ? await _smsSender.SendSmsAsync(notification.PhoneNumber, notification.Message, cancellationToken)
                : await _smsSender.SendSmsAsync(notification.PhoneNumber, notification.Message, notification.ExtraParams, cancellationToken);
            if (!result.IsSuccess)
                throw new InvalidOperationException($"Sending SMS to \"{notification.PhoneNumber}\" failed.");
        }
    }
}
