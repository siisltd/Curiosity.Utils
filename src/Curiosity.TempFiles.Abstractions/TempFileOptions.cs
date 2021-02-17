using System;
using System.Collections.Generic;
using Curiosity.Configuration;

namespace Curiosity.TempFiles
{
    /// <summary>
    /// Options for working with temporary files.
    /// </summary>
    public class TempFileOptions : ILoggableOptions, IValidatableOptions
    {
        /// <summary>
        /// Full path to temp directory.
        /// </summary>
        public string TempPath { get; set; } = "./.tmp";

        /// <inheritdoc />
        public virtual IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
        {
            var errors = new ConfigurationValidationErrorCollection();

            errors.AddErrorIf(String.IsNullOrWhiteSpace(TempPath), nameof(TempPath), "can not be empty");
            
            return errors;
        }
    }
}