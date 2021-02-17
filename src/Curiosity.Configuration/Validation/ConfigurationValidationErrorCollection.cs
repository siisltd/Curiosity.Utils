using System;
using System.Collections;
using System.Collections.Generic;

namespace Curiosity.Configuration
{
    /// <summary>
    /// Collection of configuration errors.
    /// </summary>
    /// <remarks>
    /// Useful for complex configuration validation. Encapsulates all logic for creation list of errors.
    /// </remarks>
    public class ConfigurationValidationErrorCollection : IReadOnlyCollection<ConfigurationValidationError>
    {
        private readonly List<ConfigurationValidationError> _errors;

        private readonly string? _fieldNamePrefix;

        /// <param name="fieldNamePrefix">Prefix for field name. Use for nested options</param>
        public ConfigurationValidationErrorCollection(string? fieldNamePrefix = null)
        {
            _errors = new List<ConfigurationValidationError>();
            _fieldNamePrefix = fieldNamePrefix;
        }

        /// <summary>
        /// Adds a new error to builder.
        /// </summary>
        /// <param name="fieldName">Name of field with invalid data</param>
        /// <param name="error">Validation error description</param>
        /// <exception cref="ArgumentNullException">If some of args are incorrect</exception>
        public void AddError(string fieldName, string error)
        {
            if (String.IsNullOrWhiteSpace(fieldName)) throw new ArgumentNullException(nameof(fieldName));
            if (String.IsNullOrWhiteSpace(error)) throw new ArgumentNullException(nameof(error));

            var fullFieldName = String.IsNullOrWhiteSpace(_fieldNamePrefix)
                ? fieldName
                : $"{_fieldNamePrefix}:{fieldName}";
            _errors.Add(new ConfigurationValidationError(fullFieldName, error));
        }

        /// <summary>
        /// Adds a new error to builder if specified condition if True.
        /// </summary>
        /// <param name="condition">Condition</param>
        /// <param name="fieldName">Name of field with invalid data</param>
        /// <param name="error">Validation error description</param>
        /// <exception cref="ArgumentNullException">If some of args are incorrect</exception>
        public void AddErrorIf(bool condition, string fieldName, string error)
        {
            if (String.IsNullOrWhiteSpace(fieldName)) throw new ArgumentNullException(nameof(fieldName));
            if (String.IsNullOrWhiteSpace(error)) throw new ArgumentNullException(nameof(error));

            if (condition)
            {
                var fullFieldName = String.IsNullOrWhiteSpace(_fieldNamePrefix)
                    ? fieldName
                    : $"{_fieldNamePrefix}:{fieldName}";
                _errors.Add(new ConfigurationValidationError(fullFieldName, error));
            }
        }

        /// <summary>
        /// Adds a collection of errors to builder.
        /// </summary>
        /// <param name="errors">Errors</param>
        /// <exception cref="ArgumentNullException">If arg is null</exception>
        public void AddErrors(IEnumerable<ConfigurationValidationError> errors)
        {
            if (errors == null) throw new ArgumentNullException(nameof(errors));

            _errors.AddRange(errors);
        }

        /// <summary>
        /// Adds a collection of errors to builder.
        /// </summary>
        /// <param name="condition">Condition for adding errors</param>
        /// <param name="errors">Errors</param>
        /// <exception cref="ArgumentNullException">If arg is null</exception>
        public void AddErrorsIf(bool condition, IEnumerable<ConfigurationValidationError> errors)
        {
            if (errors == null) throw new ArgumentNullException(nameof(errors));
            
            if (!condition) return;

            _errors.AddRange(errors);
        }

        #region IReadOnlyCollection

        /// <inheritdoc />
        public IEnumerator<ConfigurationValidationError> GetEnumerator()
        {
            return _errors.GetEnumerator();
        }

        /// <inheritdoc />
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <inheritdoc />
        public int Count => _errors.Count;

        #endregion
    }
}