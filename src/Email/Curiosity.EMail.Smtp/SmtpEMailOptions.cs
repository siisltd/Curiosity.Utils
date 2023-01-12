using System.Collections.Generic;
using Curiosity.Configuration;

namespace Curiosity.EMail.Smtp
{
    /// <inheritdoc />
    public class SmtpEMailOptions : ISmtpEMailOptions
    {
        /// <inheritdoc />
        public string SmtpServer { get; set; } = null!;

        /// <inheritdoc />
        public int SmtpPort { get; set; } = 25;

        /// <inheritdoc />
        public string SmtpLogin { get; set; } = null!;
        
        /// <inheritdoc />
        public string SmtpPassword { get; set; } = null!;
        
        /// <inheritdoc />
        public string EMailFrom { get; set; } = null!;
        
        /// <inheritdoc />
        public string? ReplyTo { get; set; }
        
        /// <inheritdoc />
        public string SenderName { get; set; } = null!;

        /// <summary>
        /// Should sender send email if incorrect type of extra params was passed?
        /// </summary>
        public bool IgnoreIncorrectExtraParamsType { get; set; } = false;

        /// <inheritdoc />
        public virtual IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
        {
            return SmtpEMailOptionsValidator.Validate(this, prefix);
        }
    }
}
