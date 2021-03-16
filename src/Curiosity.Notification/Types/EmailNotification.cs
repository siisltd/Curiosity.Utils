using System;
using Curiosity.Notification.Abstractions;

namespace Curiosity.Notification.Types
{
    public class EmailNotification : INotification
    {
        public const string Type = "curiosity.notification.types.email";
        public string ChannelType => Type;
        public string Email { get; }
        public string Subject { get; }
        public string Body { get; }
        public bool IsBodyHtml { get; }

        public EmailNotification(
            string email, 
            string subject, 
            string body, 
            bool isBodyHtml = false)
        {
            if (String.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(nameof(email));
            if (String.IsNullOrWhiteSpace(subject)) throw new ArgumentNullException(nameof(subject));
            if (String.IsNullOrWhiteSpace(body)) throw new ArgumentNullException(nameof(body));
            
            Email = email;
            Subject = subject;
            Body = body;
            IsBodyHtml = isBodyHtml;
        }
    }
}