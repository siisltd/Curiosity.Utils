using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace Curiosity.DataAnnotations
{
    /// <summary>
    /// Attribute for enum validation. Checks that value contains only allowed for specified enum values.
    /// </summary>
    /// <remarks>
    /// Simplifies <see cref="RangeAttribute"/> usage.
    /// </remarks>
    public class EnumRangeAttribute : RangeAttribute
    {
        public EnumRangeAttribute(int minValue, Type enumType) : base(minValue, GetMax(enumType))
        {
        }
        
        public EnumRangeAttribute(Type enumType) : base(1, GetMax(enumType))
        {
        }

        private static long GetMax(Type enumType)
        {
            if (enumType == null) throw new ArgumentNullException(nameof(enumType));
            if (!enumType.IsEnum) throw new ArgumentException($"Can't get max value for {nameof(EnumRangeAttribute)} because {enumType.Name} must be enum.");
            
            return Enum.GetValues(enumType).Cast<long>().Max();
        }
    }
}