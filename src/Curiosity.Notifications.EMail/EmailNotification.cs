using System;
using Curiosity.EMail;

namespace Curiosity.Notifications.EMail
{
    /// <summary>
    /// EMai notification.
    /// </summary>
    public class EmailNotification : INotification
    {
        /// <summary>
        /// Type of an EMail notification.
        /// </summary>
        public static readonly string Type = "curiosity.notifications.email";
        
        /// <summary>
        /// The type of channel through which the notification will be sent
        /// </summary>
        public string ChannelType => Type;
        
        /// <summary>
        /// EMail to send notification to.
        /// </summary>
        public string Email { get; }
        
        /// <summary>
        /// EMail subject.
        /// </summary>
        public string Subject { get; }
        
        /// <summary>
        /// The body of a EMail message.
        /// </summary>
        public string Body { get; }
        
        /// <summary>
        /// Is <see cref="Body"/> content present in a HTML format.
        /// </summary>
        public bool IsBodyHtml { get; }

        /// <summary>
        /// Extra params for sending EMails.
        /// </summary>
        /// <remarks>
        /// Params must be compatible with underlying EMail sender.
        /// </remarks>
        public IEMailExtraParams? ExtraParams { get; }

        public EmailNotification(
            string email, 
            string subject, 
            string body, 
            bool isBodyHtml = false,
            IEMailExtraParams? extraParams = null)
        {
            if (String.IsNullOrWhiteSpace(email)) throw new ArgumentNullException(nameof(email));
            if (String.IsNullOrWhiteSpace(subject)) throw new ArgumentNullException(nameof(subject));
            if (String.IsNullOrWhiteSpace(body)) throw new ArgumentNullException(nameof(body));
            
            Email = email;
            Subject = subject;
            Body = body;
            IsBodyHtml = isBodyHtml;

            ExtraParams = extraParams;
        }
    }
}
