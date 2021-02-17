using System;
using System.Collections.Generic;
using Curiosity.Configuration;

namespace Curiosity.Hosting
{
    /// <inheritdoc />
    public class LoggingConfigurationOptions : ILoggingConfigurationOptions
    {
        private const string CanNotBeEmptyErrorDescription = "can not be empty";

        /// <inheritdoc />
        public string LogConfigurationPath { get; set; } = "./NLog.config";

        /// <inheritdoc />
        public string LogOutputDirectory { get; set; } = "./log";

        /// <inheritdoc />
        public virtual IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
        {
            var errors = new ConfigurationValidationErrorCollection(prefix);
            if (String.IsNullOrWhiteSpace(LogConfigurationPath))
            {
                errors.AddError(nameof(LogConfigurationPath), CanNotBeEmptyErrorDescription);
            }

            return errors;
        }
    }
}