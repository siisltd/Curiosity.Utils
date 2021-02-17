using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Curiosity.DataAnnotations
{
    /// <summary>
    /// Validates object using data annotations.
    /// </summary>
    public static class DataAnnotationsValidator
    {
        /// <summary>
        /// Tries to validate specified object using data annotations.
        /// </summary>
        /// <param name="object">Object to validate.</param>
        /// <param name="results">Validation errors.</param>
        /// <returns>True if object is valid, otherwise false.</returns>
        public static bool TryValidate(object @object, out ICollection<ValidationResult> results)
        {
            var context = new ValidationContext(@object, serviceProvider: null, items: null);
            
            results = new List<ValidationResult>();
            return Validator.TryValidateObject(
                @object,
                context,
                results, 
                validateAllProperties: true
            );
        }
    }
}