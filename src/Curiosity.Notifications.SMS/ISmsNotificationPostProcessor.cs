using System.Threading;
using System.Threading.Tasks;
using Curiosity.SMS;
using Curiosity.Tools;

namespace Curiosity.Notifications.SMS
{
    /// <summary>
    /// Class for making some action after SMS notification has been sent.
    /// </summary>
    public interface ISmsNotificationPostProcessor
    {
        /// <summary>
        /// Processes specified SMS notification after notification has been sent.
        /// </summary>
        /// <param name="notification">Send EMail notification.</param>
        /// <param name="result">Result of sending <paramref name="notification"/>.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        public Task ProcessAsync(SmsNotification notification, Response<SmsSentResult> result, CancellationToken cancellationToken = default);
    }
}
