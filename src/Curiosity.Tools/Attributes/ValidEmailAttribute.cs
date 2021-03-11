using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;
using System.Text.RegularExpressions;

namespace Curiosity.Tools.Attributes
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class ValidEmailAttribute : DataTypeAttribute
    {
        public new string ErrorMessage { get; set; } = "Invalid e-mail";
        
        private const string ModelPattern = @"^(?!\.)(""([^""\r\\]|\\[""\r\\])*""|"
                                       + @"([-a-z0-9!#$%&'*+/=?^_`{|}~]|(?<!\.)\.)*)(?<!\.)"
                                       + @"@[a-z0-9][\w\.-]*[a-z0-9]\.[a-z][a-z\.]*[a-z]$";
        private static readonly Regex EmailRegex = new Regex(ModelPattern, RegexOptions.IgnoreCase);
        
        /// <summary>
        /// Checks that e-mail address is valid
        /// </summary>
        public ValidEmailAttribute() : base(DataType.EmailAddress)
        {
        }
        
        public ValidEmailAttribute([NotNull] string customDataType) : base(customDataType)
        {
        }

        public override bool IsValid(object? value)
        {
            if (value == null) return true; // attribute [Required] should checks value for null

            return EmailRegex.IsMatch(value as string ?? "");
        }

        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage;
        }
    }
}