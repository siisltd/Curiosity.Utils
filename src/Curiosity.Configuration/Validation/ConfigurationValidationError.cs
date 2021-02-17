using System;

namespace Curiosity.Configuration
{
    /// <summary>
    /// Error in configuration or options.
    /// </summary>
    public class ConfigurationValidationError
    {
        /// <summary>
        /// Name of field with invalid data.
        /// </summary>
        public string FieldName { get; }
        
        /// <summary>
        /// Validation error description.
        /// </summary>
        public string Error { get; }

        /// <summary>
        /// Adds a new error to builder.
        /// </summary>
        /// <param name="fieldName">Name of field with invalid data</param>
        /// <param name="error">Validation error description</param>
        /// <exception cref="ArgumentNullException">If some of args are incorrect</exception>
        public ConfigurationValidationError(string fieldName, string error)
        {
            if (String.IsNullOrWhiteSpace(fieldName)) throw new ArgumentNullException(nameof(fieldName));
            if (String.IsNullOrWhiteSpace(error)) throw new ArgumentNullException(nameof(error));
            
            FieldName = fieldName;
            Error = error;
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return $"{FieldName}: {Error}";
        }
    }
}