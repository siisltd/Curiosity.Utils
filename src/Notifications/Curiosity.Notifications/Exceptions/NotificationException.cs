using System;

namespace Curiosity.Notifications
{
    /// <summary>
    /// Throws when notification can't be successfully send
    /// </summary>
    public class NotificationException : Exception
    {
        /// <summary>
        /// Error code.
        /// </summary>
        public NotificationErrorCode ErrorCode { get; }

        /// <inheritdoc cref="NotificationException"/>
        public NotificationException(NotificationErrorCode errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

        /// <inheritdoc cref="NotificationException"/>
        public NotificationException(NotificationErrorCode errorCode, string message, Exception innerException) : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }
}
