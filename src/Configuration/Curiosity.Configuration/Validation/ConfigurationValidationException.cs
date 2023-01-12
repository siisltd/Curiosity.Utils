using System;
using System.Collections.Generic;
using System.Linq;

namespace Curiosity.Configuration
{
    /// <summary>
    /// Exception raised when configuration validation failed.
    /// </summary>
    public class ConfigurationValidationException : Exception
    {
        /// <summary>
        /// Information about configuration errors.
        /// </summary>
        public IReadOnlyCollection<ConfigurationValidationError> Errors { get; }

        /// <inheritdoc />
        public ConfigurationValidationException(string message, IEnumerable<ConfigurationValidationError>? errors = null)
        {
            if (String.IsNullOrWhiteSpace(message)) throw new ArgumentNullException(nameof(message));
            
            Errors = errors != null
                ? errors.ToArray() as IReadOnlyCollection<ConfigurationValidationError>
                : Array.Empty<ConfigurationValidationError>();
        }
    }
}