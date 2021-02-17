using System;
using System.Text;

namespace Curiosity.Configuration
{
    /// <summary>
    /// Extension methods for <see cref="IValidatableOptions"/>.
    /// </summary>
    public static class ValidatableOptionsExtensions
    {
        /// <summary>
        /// Validates options and throws exception on errors.
        /// </summary>
        /// <param name="options">Options to validate</param>
        /// <param name="prefix">Options prefix</param>
        /// <exception cref="ConfigurationValidationException">If options are invalid</exception>
        public static void AssertValid(this IValidatableOptions options, string? prefix = null)
        {
            var errors = options.Validate(prefix);
            if (errors.Count == 0) return;
            
            var optionName = String.IsNullOrWhiteSpace(prefix)
                ? " "
                : $"{prefix} ";
            
            var errorsBuilder = new StringBuilder();
            foreach (var validationError in errors)
            {
                errorsBuilder.AppendLine($"{validationError.FieldName}: {validationError.Error}");
            }
            
            var message = $"Option {optionName}are invalid. Errors: {Environment.NewLine}{errorsBuilder}";
            throw new ConfigurationValidationException(message, errors);
        }
    }
}