using System;

namespace Curiosity.SMS
{
    /// <summary>
    /// Result of sending SMS.
    /// </summary>
    public class SmsSentResult
    {
        /// <summary>
        /// Total amount of sent SMS.
        /// </summary>
        public int? SmsCount { get; }

        /// <summary>
        /// Cost of sending this SMS notification.
        /// </summary>
        public decimal? Cost { get; }

        /// <summary>
        /// Response from remote SMS service.
        /// </summary>
        /// <remarks>
        /// Useful for debugging.
        /// </remarks>
        public string ResponseJson { get; }

        public SmsSentResult(int? smsCount, decimal? cost, string responseJson)
        {
            if (smsCount < 0) throw new ArgumentOutOfRangeException(nameof(smsCount));
            if (String.IsNullOrWhiteSpace(responseJson)) throw new ArgumentNullException(nameof(responseJson));

            SmsCount = smsCount;
            Cost = cost;
            ResponseJson = responseJson;
        }
    }
}
