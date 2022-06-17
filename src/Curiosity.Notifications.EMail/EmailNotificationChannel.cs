using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.EMail;
using Curiosity.Tools;
using Microsoft.Extensions.Logging;

namespace Curiosity.Notifications.EMail
{
    /// <summary>
    /// <inheritdoc cref="IEmailNotificationChannel"/>
    /// Uses registered at IoC implementation of <see cref="IEMailSender"/> to send notifications.
    /// </summary>
    public class EmailNotificationChannel : NotificationChannelBase<EmailNotification>, IEmailNotificationChannel
    {
        private readonly IEMailSender _sender;

        private readonly IReadOnlyList<IEMailNotificationPostProcessor> _postProcessors;

        /// <inheritdoc cref="EmailNotificationChannel"/>
        public EmailNotificationChannel(
            ILogger<EmailNotificationChannel> logger,
            IEMailSender sender,
            IEnumerable<IEMailNotificationPostProcessor> postProcessors) : base(logger, EmailNotification.Type)
        {
            _sender = sender ?? throw new ArgumentNullException(nameof(sender));

            if (postProcessors == null) throw new ArgumentNullException(nameof(postProcessors));
            _postProcessors = postProcessors.ToArray();
        }

        /// <inheritdoc />
        protected override async Task ProcessNotificationAsync(EmailNotification notification, CancellationToken cancellationToken = default)
        {
            if (notification == null) throw new ArgumentNullException(nameof(notification));

            // send EMail
            Response result;
            try
            {
                result = notification.ExtraParams == null
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
            }
            catch (Exception e)
            {
                throw new NotificationException(NotificationErrorCode.Unknown, "Unknown error while sending email notification", e);
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
                    Logger.LogError(e, $"Error while post processing EMail notification. Reason: {e.Message}");
                }
            }

            // throw exception if something failed
            if (!result.IsSuccess)
            {
                var error = result.Errors.FirstOrDefault() ?? new Error((int) EmailError.Unknown, "Unknown email error");
                var errorCode = (EmailError) error.Code;
                var notificationCode = errorCode switch
                {
                    EmailError.Auth => NotificationErrorCode.Auth,
                    EmailError.Communication => NotificationErrorCode.Communication,
                    EmailError.RateLimit => NotificationErrorCode.RateLimit,
                    EmailError.NoMoney => NotificationErrorCode.NoMoney,
                    EmailError.IncorrectRequestData => NotificationErrorCode.IncorrectRequestData,
                    _ => NotificationErrorCode.Unknown,
                };
                
                throw new NotificationException(notificationCode, error.Description);
            }
        }
    }
}
