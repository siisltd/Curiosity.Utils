using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.SMS;
using Curiosity.Tools;
using Microsoft.Extensions.Logging;

namespace Curiosity.Notifications.SMS
{
    public class SmsNotificationChannel : NotificationChannelBase<SmsNotification>
    {
        private readonly ISmsSender _smsSender;

        private readonly IReadOnlyList<ISmsNotificationPostProcessor> _postProcessors;

        public SmsNotificationChannel(
            ILogger<SmsNotificationChannel> logger,
            ISmsSender smsSender,
            IEnumerable<ISmsNotificationPostProcessor> postProcessors) : base(logger, SmsNotification.Type)
        {
            if (postProcessors == null) throw new ArgumentNullException(nameof(postProcessors));
            _smsSender = smsSender ?? throw new ArgumentNullException(nameof(smsSender));

            _postProcessors = postProcessors.ToArray();
        }

        protected override async Task ProcessNotificationAsync(SmsNotification notification, CancellationToken cancellationToken = default)
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));

            // send SMS
            var result = notification.ExtraParams == null
                ? await _smsSender.SendSmsAsync(notification.PhoneNumber, notification.Message, cancellationToken)
                : await _smsSender.SendSmsAsync(notification.PhoneNumber, notification.Message, notification.ExtraParams, cancellationToken);

            // execute post processing
            for (var i = 0; i < _postProcessors.Count; i++)
            {
                var postProcessor = _postProcessors[i];

                try
                {
                    await postProcessor.ProcessAsync(notification, result, cancellationToken);
                }
                catch (Exception e)
                {
                    Logger.LogError(e, $"Error while post processing SMS notification. Reason: {e.Message}");
                }
            }

            // throw exception if something failed
            if (!result.IsSuccess)
            {
                var error = result.Errors.FirstOrDefault() ?? new Error((int)SmsError.Unknown, "Unknown email error");
                throw new InvalidOperationException($"Sending email to \"{notification.PhoneNumber}\" failed. SmsResult={error.Code}", new Exception(error.Description));
            }
        }
    }
}
