using System;
using System.Collections.Generic;
using Curiosity.Configuration;

namespace Curiosity.SMS.Smsc
{
    /// <summary>
    /// Options for <see cref="Curiosity.SMS.Smsc.SmscSender"/>
    /// </summary>
    public class SmscOptions : IValidatableOptions, ILoggableOptions
    {
        private const string CanNotBeEmptyErrorDescription = "can not be empty";

        /// <summary>
        /// Login for Smsc.
        /// </summary>
        public string SmscLogin { get; set; } = null!;

        /// <summary>
        /// Pass for Smsc.
        /// </summary>
        public string SmscPassword { get; set; } = null!;

        /// <summary>
        /// Sender's name.
        /// </summary>
        public string? SmscSender { get; set; } = null!;

        /// <inheritdoc />
        public IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
        {
            var errors = new ConfigurationValidationErrorCollection(prefix);
            errors.AddErrorIf(String.IsNullOrEmpty(SmscLogin), nameof(SmscLogin), CanNotBeEmptyErrorDescription);
            errors.AddErrorIf(String.IsNullOrEmpty(SmscPassword), nameof(SmscPassword), CanNotBeEmptyErrorDescription);

            return errors;
        }
    }
}
