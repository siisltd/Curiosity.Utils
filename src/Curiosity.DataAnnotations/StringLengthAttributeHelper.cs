using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Curiosity.DataAnnotations
{
    /// <summary>
    /// Useful methods for<see cref="StringLengthAttribute"/>.
    /// </summary>
    public static class StringLengthAttributeHelper
    {
        /// <summary>
        /// Returns max string value for property or <see langword="null"/> if attribute <see cref="StringLengthAttribute"/> isn't used.
        /// </summary>
        public static int? GetPropertyMaxLength<T>(this T _, string propertyName)
        {
            if (String.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException(nameof(propertyName));
            
            var property = typeof(T)
                .GetProperty(propertyName);
            if (property == null) throw new ArgumentException($"Property {propertyName} not found", propertyName);
            
            var stringLengthAttribute = property.GetCustomAttributes(typeof(StringLengthAttribute), false)
                .Cast<StringLengthAttribute>()
                .FirstOrDefault();
            
            return stringLengthAttribute?.MaximumLength;
        }
    }
}