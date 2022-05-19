using System;
using System.Threading;
using System.Threading.Tasks;
using MailKit.Net.Smtp;
using MailKit.Security;
using Curiosity.Configuration;
using Curiosity.Tools;
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

        /// <inheritdoc cref="SmtpEMailSender"/>
        public SmtpEMailSender(
            ISmtpEMailOptions smtpEMailOptions,
            ILogger<SmtpEMailSender> logger)
        {
            _emailOptions = smtpEMailOptions ?? throw new ArgumentNullException(nameof(smtpEMailOptions));
            _emailOptions.AssertValid();
            
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public Task<Response> SendAsync(string toAddress, string subject, string body, bool isBodyHtml = false, CancellationToken cancellationToken = default)
        {
            EmailGuard.AssertToAddress(toAddress);
            EmailGuard.AssertSubject(subject);
            EmailGuard.AssertBody(body);

            var senderName = _emailOptions.SenderName.Trim();
            var emailFrom = _emailOptions.EMailFrom.Trim();
            var replyTo = _emailOptions.ReplyTo?.Trim();

            return SendAsync(toAddress, subject, body, isBodyHtml, senderName, emailFrom, replyTo, cancellationToken);
        }

        private async Task<Response> SendAsync(
            string toAddress,
            string subject,
            string body,
            bool isBodyHtml,
            string senderName,
            string emailFrom,
            string? replyTo,
            CancellationToken cancellationToken = default)
        {
            _logger.LogDebug("Sending EMail to \"{ToAddress}\" with the subject \"{Subject}\", text.Length = {BodyTextLength}",
                toAddress,
                subject,
                body.Length);

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

                    _logger.LogTrace("Connecting to SMTP server (Host={Host}, Port={Port})...", _emailOptions.SmtpServer, _emailOptions.SmtpPort);
                    await smtpClient.ConnectAsync(
                        _emailOptions.SmtpServer,
                        _emailOptions.SmtpPort,
                        SecureSocketOptions.Auto,
                        cancellationToken);

                    _logger.LogTrace("Authenticating to SMTP server (Host={Host}, Port={Port})...", _emailOptions.SmtpServer, _emailOptions.SmtpPort);
                    await smtpClient.AuthenticateAsync(_emailOptions.SmtpLogin, _emailOptions.SmtpPassword, cancellationToken);

                    _logger.LogTrace("Sending email to SMTP server (Host={Host}, Port={Port})...", _emailOptions.SmtpServer, _emailOptions.SmtpPort);
                    await smtpClient.SendAsync(message, cancellationToken);

                    _logger.LogTrace("Disconnecting from SMTP server (Host={Host}, Port={Port})...", _emailOptions.SmtpServer, _emailOptions.SmtpPort);
                    await smtpClient.DisconnectAsync(true, cancellationToken);
                }

                _logger.LogInformation("EMail message was successfully sent to \"{ToAddress}\"", toAddress);

                return Response.Successful();
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error sending EMail message to \"{ToAddress}\"", toAddress);

                //todo analyze exceptions and return more specified EmailError
                return Response.Failed(new Error((int) EmailError.Unknown, e.Message));
            }
        }

        /// <inheritdoc />
        public Task<Response> SendAsync(string toAddress, string subject, string body, bool isBodyHtml, IEMailExtraParams emailExtraParams, CancellationToken cancellationToken = default)
        {
            EmailGuard.AssertToAddress(toAddress);
            EmailGuard.AssertSubject(subject);
            EmailGuard.AssertBody(body);

            if (emailExtraParams == null) throw new ArgumentNullException(nameof(emailExtraParams));

            SmtpEMailExtraParams? smtpEMailExtraParams = null;
            if (emailExtraParams is SmtpEMailExtraParams @params)
            {
                smtpEMailExtraParams = @params;
            }
            else
            {
                if (!_emailOptions.IgnoreIncorrectExtraParamsType)
                    throw new ArgumentException($"Only {typeof(SmtpEMailExtraParams)} is supported for this sender.", nameof(emailExtraParams));
            }

            var senderName = smtpEMailExtraParams?.SenderName?.Trim() ?? _emailOptions.SenderName.Trim();
            var emailFrom = smtpEMailExtraParams?.EMailFrom?.Trim() ?? _emailOptions.EMailFrom.Trim();
            var replyTo = smtpEMailExtraParams?.ReplyTo?.Trim() ?? _emailOptions.ReplyTo?.Trim();

            return SendAsync(toAddress, subject, body, isBodyHtml, senderName, emailFrom, replyTo, cancellationToken);
        }
    }
}
