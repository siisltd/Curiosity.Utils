using System;
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
        public async Task<bool> SendAsync(string toAddress, string subject, string body, bool isBodyHtml = false)
        {
            if (String.IsNullOrWhiteSpace(toAddress))
                throw new ArgumentNullException(nameof(toAddress));
            if (String.IsNullOrWhiteSpace(subject))
                throw new ArgumentNullException(nameof(subject));
            if (String.IsNullOrWhiteSpace(body))
                throw new ArgumentNullException(nameof(body));

            _logger.LogInformation("Sending EMail to {0} with the subject \"{1}\", text.Length = {2}", 
                toAddress,
                subject,
                body.Length);

            var hasError = false;
            try
            {
                var message = new MimeMessage();
                message.To.Add(MailboxAddress.Parse(toAddress.Trim()));
                message.From.Add(new MailboxAddress(_emailOptions.SenderName.Trim(), _emailOptions.EMailFrom.Trim()));

                if (!String.IsNullOrWhiteSpace(_emailOptions.ReplyTo))
                {
                    message.ReplyTo.Add(new MailboxAddress(_emailOptions.SenderName.Trim(), _emailOptions.ReplyTo.Trim()));
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
                        SecureSocketOptions.Auto);

                    await smtpClient.AuthenticateAsync(_emailOptions.SmtpLogin, _emailOptions.SmtpPassword);
                    await smtpClient.SendAsync(message);
                    await smtpClient.DisconnectAsync(true);
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
    }
}