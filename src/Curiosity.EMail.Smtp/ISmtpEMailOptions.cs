using Curiosity.Configuration;

namespace Curiosity.EMail.Smtp
{
    /// <summary>
    /// Options for sending EMail via SMTP.
    /// </summary>
    public interface ISmtpEMailOptions : ILoggableOptions, IValidatableOptions
    {
        /// <summary>
        /// SMTP server host.
        /// </summary>
        string SmtpServer { get; }

        /// <summary>
        /// SMTP server port.
        /// </summary>
        public int SmtpPort { get; }

        /// <summary>
        /// SMTP account user name.
        /// </summary>
        string SmtpLogin { get; }
        
        /// <summary>
        /// SMTP account password.
        /// </summary>
        string SmtpPassword { get; }
        
        /// <summary>
        /// Sender's EMail.
        /// </summary>
        string EMailFrom { get; }
        
        /// <summary>
        /// Reply address for EMail.
        /// </summary>
        string? ReplyTo { get; }
        
        /// <summary>
        /// Human readable sender name.
        /// </summary>
        string SenderName { get; }
    }
}