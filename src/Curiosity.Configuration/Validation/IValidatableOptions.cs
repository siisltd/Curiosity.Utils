using System.Collections.Generic;

namespace Curiosity.Configuration
{
    /// <summary>
    /// Validatable options.
    /// </summary>
    public interface IValidatableOptions
    {
        /// <summary>
        /// Validates options and returns collection of errors.
        /// </summary>
        /// <returns>Collection of errors. If option is valid, collection will be empty</returns>
        IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null);
    }
}