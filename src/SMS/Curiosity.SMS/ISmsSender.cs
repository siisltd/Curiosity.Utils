using System.Threading;
using System.Threading.Tasks;
using Curiosity.Tools;

namespace Curiosity.SMS
{
    /// <summary>
    /// Service for sending SMS messages.
    /// </summary>
    public interface ISmsSender
    {
        /// <summary>
        /// Sends SMS.
        /// </summary>
        /// <param name="phoneNumber">Phone number to send SMS.</param>
        /// <param name="message">SMS text.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of sending.</returns>
        Task<Response<SmsSentResult>> SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default);

        /// <summary>
        /// Sends SMS.
        /// </summary>
        /// <param name="phoneNumber">Phone number to send SMS.</param>
        /// <param name="message">SMS text.</param>
        /// <param name="extraParams">Extra params for sendin SMS.</param>
        /// <param name="cancellationToken">Cancellation token</param>
        /// <returns>Result of sending.</returns>
        Task<Response<SmsSentResult>> SendSmsAsync(string phoneNumber, string message, ISmsExtraParams extraParams, CancellationToken cancellationToken = default);
    }
}
