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

        private const string ZeroOrNegativeErrorDescription = "should be greater than 0";

        /// <summary>
        /// Login for Smsc.
        /// </summary>
        public string SmscLogin { get; set; }

        /// <summary>
        /// Pass for Smsc.
        /// </summary>
        public string SmscPassword { get; set; }

        /// <summary>
        /// Sender's name.
        /// </summary>
        public string SmscSender { get; set; }

        /// <inheritdoc />
        public IReadOnlyCollection<ConfigurationValidationError> Validate(string prefix = null)
        {
            var errors = new ConfigurationValidationErrorCollection(prefix);
            errors.AddErrorIf(String.IsNullOrEmpty(SmscLogin), nameof(SmscLogin), CanNotBeEmptyErrorDescription);
            errors.AddErrorIf(String.IsNullOrEmpty(SmscPassword), nameof(SmscPassword), CanNotBeEmptyErrorDescription);
            errors.AddErrorIf(String.IsNullOrEmpty(SmscSender), nameof(SmscSender), CanNotBeEmptyErrorDescription);

            return errors;
        }
    }
}
