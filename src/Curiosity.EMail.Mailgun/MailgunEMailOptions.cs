using System;
using System.Collections.Generic;
using Curiosity.Configuration;

namespace Curiosity.EMail.Mailgun
{
    /// <summary>
    /// Options which necessary for a MailGun work.
    /// </summary>
    public class MailgunEMailOptions :
        IValidatableOptions,
        ILoggableOptions
    {
        /// <summary>
        /// Mailgun user.
        /// </summary>
        public string MailgunUser { get; set; } = null!;

        /// <summary>
        /// API key for a MailGun work.
        /// </summary>
        public string MailGunApiKey { get; set; } = null!;

        /// <summary>
        /// Domain of MailGun
        /// </summary>
        public string MailGunDomain { get; set; } = null!;

        /// <summary>
        /// Sender's EMail
        /// </summary>
        public string EMailFrom { get; set; } = null!;

        /// <inheritdoc />
        public virtual IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
        {
            var errors = new ConfigurationValidationErrorCollection(prefix);

            errors.AddErrorIf(String.IsNullOrWhiteSpace(MailGunApiKey), nameof(MailGunApiKey), "can not be empty");
            errors.AddErrorIf(String.IsNullOrWhiteSpace(MailGunDomain), nameof(MailGunDomain), "can not be empty");
            errors.AddErrorIf(String.IsNullOrWhiteSpace(MailgunUser), nameof(MailgunUser), "can not be empty");
            errors.AddErrorIf(String.IsNullOrWhiteSpace(EMailFrom), nameof(EMailFrom), "can not be empty");

            return errors;
        }
    }
}
