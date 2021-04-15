using System;
using Curiosity.Notifications;

namespace Curiosity.Notifications.SMS
{
    /// <summary>
    /// SMS notification.
    /// </summary>
    public class SmsNotification : INotification
    {
        /// <summary>
        /// Type of a SMS notification.
        /// </summary>
        public static readonly string Type = "curiosity.notifications.sms";
        
        /// <summary>
        /// The type of channel through which the notification will be sent
        /// </summary>
        public string ChannelType => Type;
        
        /// <summary>
        /// Phone number to send notification to. 
        /// </summary>
        public string PhoneNumber { get; }
        
        /// <summary>
        /// Content of an SMS.
        /// </summary>
        public string Message { get; }

        public SmsNotification(
            string phoneNumber, 
            string message)
        {
            if (String.IsNullOrWhiteSpace(phoneNumber)) throw new ArgumentNullException(nameof(phoneNumber));
            if (String.IsNullOrWhiteSpace(message)) throw new ArgumentNullException(nameof(message));

            PhoneNumber = phoneNumber;
            Message = message;
        }
    }
}