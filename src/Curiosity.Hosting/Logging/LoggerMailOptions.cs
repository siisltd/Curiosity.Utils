using System;
using System.Collections.Generic;
using Curiosity.Configuration;

namespace Curiosity.Hosting
{
    /// <inheritdoc cref="ILoggerMailOptions" />
    public class LoggerMailOptions : 
        ILoggerMailOptions
    {
        private const string CanNotBeEmptyErrorDescription = "can not be empty";
        
        /// <inheritdoc />
        public string SmtpServer { get; set; } = null!;

        /// <inheritdoc />
        public int SmtpPort { get; set; } = 465;
        
        /// <inheritdoc />
        public string SmtpLogin { get; set; } = null!;
        
        /// <inheritdoc />
        public string SmtpPassword { get; set; } = null!;
        
        /// <inheritdoc />
        public string EMailFrom { get; set; } = null!;
        
        /// <inheritdoc />
        public string MailTo { get; set; } = null!;
        
        public virtual IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
        {
            var errors = new ConfigurationValidationErrorCollection(prefix);
            
            errors.AddErrorIf(String.IsNullOrWhiteSpace(SmtpServer), nameof(SmtpServer),"can't be null or empty");
            errors.AddErrorIf(String.IsNullOrWhiteSpace(SmtpLogin), nameof(SmtpLogin),"can't be null or empty");
            errors.AddErrorIf(String.IsNullOrWhiteSpace(SmtpPassword), nameof(SmtpPassword),"can't be null or empty");
            errors.AddErrorIf(String.IsNullOrWhiteSpace(EMailFrom), nameof(EMailFrom),"can't be null or empty");
            errors.AddErrorIf(String.IsNullOrWhiteSpace(MailTo), nameof(MailTo),"can't be null or empty");
            errors.AddErrorIf(SmtpPort <= 0, nameof(SmtpPort),"can't be less than 1");
            
            return errors;
        }
    }
}