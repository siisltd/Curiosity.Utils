using System;
using Curiosity.Notification.Abstractions;

namespace Curiosity.Notification.Types
{
    public class SmsNotification : INotification
    {
        public const string Type = "curiosity.notification.types.sms";
        public string ChannelType => Type;
        public string PhoneNumber { get; }
        public string Message { get; }

        public SmsNotification(
            string phoneNumber, 
            string message)
        {
            if (String.IsNullOrWhiteSpace(message)) throw new ArgumentNullException(nameof(message));

            PhoneNumber = phoneNumber ?? throw new ArgumentNullException(nameof(phoneNumber));
            Message = message;
        }
    }
}