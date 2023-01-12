using System;
using System.Collections.Generic;
using Curiosity.Configuration;

namespace Curiosity.Email.UnisenderGo
{
    /// <summary>
    /// Options for sending emails via UnisenderGo.
    /// </summary>
    public class UnisenderGoEmailOptions : ILoggableOptions, IValidatableOptions
    {
        /// <summary>
        /// API key.
        /// </summary>
        public string ApiKey { get; set; } = null!;

        /// <summary>
        /// Region of API.
        /// </summary>
        public UnisenderGoRegion Region { get; set; } = UnisenderGoRegion.Russia;

        /// <summary>
        /// Sender's EMail
        /// </summary>
        public string EmailFrom { get; set; } = null!;

        /// <summary>
        /// Sender's name.
        /// </summary>
        public string FromName { get; set; } = null!;

        /// <summary>
        /// Should sender send email if incorrect type of extra params was passed?
        /// </summary>
        public bool IgnoreIncorrectExtraParamsType { get; set; } = false;

        /// <summary>
        /// Reply address for EMail.
        /// </summary>
        public string? ReplyTo { get; set; }

        /// <summary>
        /// Link for user's unsubscribe page.
        /// </summary>
        public string? UnsubscribeUrl { get; set; }

        /// <summary>
        /// Is tracking opening links enabled?
        /// </summary>
        public bool? TrackLinks { get; set; }
        
        /// <summary>
        /// Is tracking email reading enabled?
        /// </summary>
        public bool? TrackReads { get; set; }
        
        /// <summary>
        /// Should Unisender add their footer with unsubscribed url.
        /// </summary>
        /// <remarks>
        /// This option can not affect on anything if Unisender doesn't activate disabling footer for client.
        /// </remarks>
        public bool? SkipUnisenderUnsubscribeFooter { get; set; }

        /// <inheritdoc />
        public virtual IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
        {
            var errors = new ConfigurationValidationErrorCollection(prefix);

            errors.AddErrorIf(String.IsNullOrWhiteSpace(ApiKey), nameof(ApiKey), "can't be empty");
            errors.AddErrorIf(String.IsNullOrWhiteSpace(EmailFrom), nameof(EmailFrom), "can't be empty");
            errors.AddErrorIf(String.IsNullOrWhiteSpace(FromName), nameof(EmailFrom), "can't be empty");
            errors.AddErrorIf(!Enum.IsDefined(typeof(UnisenderGoRegion), Region), nameof(Region), "unknown value");

            return errors;
        }
    }
}
