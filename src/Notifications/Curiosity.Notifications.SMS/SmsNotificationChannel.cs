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
    /// <inheritdoc cref="SmsNotificationChannel"/>
    /// <remarks>
    /// Uses default implementation of <see cref="ISmsSender"/> to sent SMS.
    /// </remarks>
    public class SmsNotificationChannel : NotificationChannelBase<SmsNotification>, ISmsNotificationChannel
    {
        private readonly ISmsSender _smsSender;

        private readonly IReadOnlyList<ISmsNotificationPostProcessor> _postProcessors;

        /// <inheritdoc cref="SmsNotificationChannel"/>
        public SmsNotificationChannel(
            ILogger<SmsNotificationChannel> logger,
            ISmsSender smsSender,
            IEnumerable<ISmsNotificationPostProcessor> postProcessors) : base(logger, SmsNotification.Type)
        {
            if (postProcessors == null) throw new ArgumentNullException(nameof(postProcessors));
            _smsSender = smsSender ?? throw new ArgumentNullException(nameof(smsSender));

            _postProcessors = postProcessors.ToArray();
        }

        /// <inheritdoc />
        protected override async Task ProcessNotificationAsync(SmsNotification notification, CancellationToken cancellationToken = default)
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));

            // send SMS
            Response<SmsSentResult> result;
            try
            {
                result = notification.ExtraParams == null
                    ? await _smsSender.SendSmsAsync(notification.PhoneNumber, notification.Message, cancellationToken)
                    : await _smsSender.SendSmsAsync(notification.PhoneNumber, notification.Message, notification.ExtraParams, cancellationToken);
            }
            catch (Exception e)
            {
                throw new NotificationException(NotificationErrorCode.Unknown, "Unexpected error while sending SMS", e);
            }

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
                var error = result.Errors.FirstOrDefault() ?? new Error((int) SmsError.Unknown, "Unknown email error");
                var notificationCode = error.Code switch
                {
                    1 => NotificationErrorCode.Auth,
                    2 => NotificationErrorCode.Communication,
                    4 => NotificationErrorCode.RateLimit,
                    5 => NotificationErrorCode.NoMoney,
                    _ => NotificationErrorCode.Unknown,
                };
                
                throw new NotificationException(notificationCode, error.Description);
            }
        }
    }
}
