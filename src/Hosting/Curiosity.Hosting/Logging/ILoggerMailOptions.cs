using Curiosity.Configuration;

namespace Curiosity.Hosting
{
    /// <summary>
    /// Configuration for logging via EMail.
    /// </summary>
    public interface ILoggerMailOptions : 
        ILoggableOptions,
        IValidatableOptions
    {
        /// <summary>
        /// SMTP server host.
        /// </summary>
        string SmtpServer { get; }
        
        /// <summary>
        /// SMTP server port.
        /// </summary>
        int SmtpPort { get; set; }
        
        /// <summary>
        /// Smtp server login (username).
        /// </summary>
        string SmtpLogin { get; }
        
        /// <summary>
        /// Smtp server password.
        /// </summary>
        string SmtpPassword { get; }
        
        /// <summary>
        /// Email`s "From" field value.
        /// </summary>
        string EMailFrom { get; }
        
        /// <summary>
        /// Where to send mail to.
        /// </summary>
        string MailTo { get; }
    }
}