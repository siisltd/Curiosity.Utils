using System;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Curiosity.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using MimeKit.Text;

namespace Curiosity.EMail.Smtp
{
    /// <inheritdoc />
    public class SmtpEMailSender : ISmtpEMailSender
    {
        private readonly ISmtpEMailOptions _emailOptions;
        private readonly ILogger _logger;

        public SmtpEMailSender(
            ISmtpEMailOptions smtpEMailOptions,
            ILogger<SmtpEMailSender> logger)
        {
            _emailOptions = smtpEMailOptions ?? throw new ArgumentNullException(nameof(smtpEMailOptions));
            _emailOptions.AssertValid();
            
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public Task<bool> SendAsync(string toAddress, string subject, string body, bool isBodyHtml = false, CancellationToken cancellationToken = default)
        {
            if (String.IsNullOrWhiteSpace(toAddress))
                throw new ArgumentNullException(nameof(toAddress));
            if (String.IsNullOrWhiteSpace(subject))
                throw new ArgumentNullException(nameof(subject));
            if (String.IsNullOrWhiteSpace(body))
                throw new ArgumentNullException(nameof(body));

            var senderName = _emailOptions.SenderName.Trim();
            var emailFrom = _emailOptions.EMailFrom.Trim();
            var replyTo = _emailOptions.ReplyTo?.Trim();

            return SendAsync(toAddress, subject, body, isBodyHtml, senderName, emailFrom, replyTo, cancellationToken);
        }

        private async Task<bool> SendAsync(
            string toAddress,
            string subject,
            string body,
            bool isBodyHtml,
            string senderName,
            string emailFrom,
            string? replyTo,
            CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Sending EMail to {0} with the subject \"{1}\", text.Length = {2}",
                toAddress,
                subject,
                body.Length);

            var hasError = false;
            try
            {
                var message = new MimeMessage();
                message.To.Add(MailboxAddress.Parse(toAddress.Trim()));
                message.From.Add(new MailboxAddress(senderName, emailFrom));

                if (!String.IsNullOrWhiteSpace(replyTo))
                {
                    message.ReplyTo.Add(new MailboxAddress(senderName, replyTo.Trim()));
                }

                message.Subject = subject;
                var bodyFormat = isBodyHtml
                    ? TextFormat.Html
                    : TextFormat.Text;

                message.Body = new TextPart(bodyFormat)
                {
                    Text = body
                };

                using (var smtpClient = new SmtpClient())
                {
                    smtpClient.ServerCertificateValidationCallback = (sender, certificate, chain, errors) => true;
                    smtpClient.CheckCertificateRevocation = false;

                    await smtpClient.ConnectAsync(
                        _emailOptions.SmtpServer,
                        _emailOptions.SmtpPort,
                        SecureSocketOptions.Auto,
                        cancellationToken);

                    await smtpClient.AuthenticateAsync(_emailOptions.SmtpLogin, _emailOptions.SmtpPassword, cancellationToken);
                    await smtpClient.SendAsync(message, cancellationToken);
                    await smtpClient.DisconnectAsync(true, cancellationToken);
                }

                _logger.LogInformation("EMail message was successfully sent to {0}", toAddress);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Error sending EMail message to {0}. Reason: {e.Message}", toAddress);
                hasError = true;
            }

            return !hasError;
        }

        public Task<bool> SendAsync(string toAddress, string subject, string body, bool isBodyHtml, IEMailExtraParams emailExtraParams, CancellationToken cancellationToken = default)
        {
            if (String.IsNullOrWhiteSpace(toAddress))
                throw new ArgumentNullException(nameof(toAddress));
            if (String.IsNullOrWhiteSpace(subject))
                throw new ArgumentNullException(nameof(subject));
            if (String.IsNullOrWhiteSpace(body))
                throw new ArgumentNullException(nameof(body));

            if (emailExtraParams == null) throw new ArgumentNullException(nameof(emailExtraParams));
            if (!(emailExtraParams is SmtpEMailExtraParams smtpEMailExtraParams))
                throw new ArgumentException($"Only {typeof(SmtpEMailExtraParams)} is supported.", nameof(emailExtraParams));

            var senderName = smtpEMailExtraParams.SenderName?.Trim() ?? _emailOptions.SenderName.Trim();
            var emailFrom = smtpEMailExtraParams.EMailFrom?.Trim() ?? _emailOptions.EMailFrom.Trim();
            var replyTo = smtpEMailExtraParams.ReplyTo?.Trim() ?? _emailOptions.ReplyTo?.Trim();

            return SendAsync(toAddress, subject, body, isBodyHtml, senderName, emailFrom, replyTo, cancellationToken);
        }
    }
}
