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
        
        /// <summary>
        /// How often the temp directory will be cleaning
        /// </summary>
        public int CleaningFrequencyHours { get; set; } = 24;
        
        /// <summary>
        /// Files lifetime in hours
        /// </summary>
        public int FileTtlHours { get; set; } = 24;

        /// <inheritdoc />
        public virtual IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
        {
            var errors = new ConfigurationValidationErrorCollection();

            errors.AddErrorIf(String.IsNullOrWhiteSpace(TempPath), nameof(TempPath), "can not be empty");
            errors.AddErrorIf(CleaningFrequencyHours < 1, nameof(CleaningFrequencyHours), "Can't be less than 1");
            errors.AddErrorIf(FileTtlHours < 1, nameof(FileTtlHours), "Can't be less than 1");
            
            return errors;
        }
    }
}