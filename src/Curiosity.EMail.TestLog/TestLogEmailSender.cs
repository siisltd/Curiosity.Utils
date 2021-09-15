using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Curiosity.EMail.TestLog
{
    /// <summary>
    /// Use that sender for debugging email sending without real send
    /// </summary>
    public class TestLogEmailSender : IEMailSender
    {
        private readonly ILogger _logger;

        ///
        public TestLogEmailSender(ILogger<TestLogEmailSender> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <inheritdoc />
        public Task<bool> SendAsync(
            string toAddress,
            string subject,
            string body,
            bool isBodyHtml,
            IEMailExtraParams emailExtraParams,
            CancellationToken cancellationToken = default)
        {
            return SendAsync(toAddress, subject, body, isBodyHtml, cancellationToken);
        }
        
        public Task<bool> SendAsync(
            string toAddress,
            string subject,
            string body,
            bool isBodyHtml = false,
            CancellationToken cancellationToken = default)
        {
            _logger.LogWarning("The email was not sent, but just wrote to the log: \n" +
                               $"\tToAddress: \"{toAddress}\", \n" +
                               $"\tSubject: \"{subject}\", \n" +
                               $"\tIsBodyHtml: {isBodyHtml}, \n" +
                               $"\tBody: \"{body}\"");

            return Task.FromResult(true);
        }
    }
}
