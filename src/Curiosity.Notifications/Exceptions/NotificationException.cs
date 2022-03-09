using System;

namespace Curiosity.Notifications
{
    /// <summary>
    /// Throws when notification can't successfully send
    /// </summary>
    public class NotificationException : Exception
    {
        public NotificationErrorCode ErrorCode { get; }

        public NotificationException(NotificationErrorCode errorCode, string message) : base(message)
        {
            ErrorCode = errorCode;
        }

        public NotificationException(NotificationErrorCode errorCode, string message, Exception innerException) : base(message, innerException)
        {
            ErrorCode = errorCode;
        }
    }
}