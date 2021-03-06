﻿using System;
using System.ComponentModel.DataAnnotations;

namespace Curiosity.EMail
{
    /// <summary>
    /// Checks that e-mail address is valid.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field | AttributeTargets.Parameter, AllowMultiple = false)]
    public class ValidEmailAttribute : DataTypeAttribute
    {
        /// <inheritdoc cref="DataTypeAttribute" />
        public new string ErrorMessage { get; set; } = "Invalid e-mail";
        
        /// <summary>
        /// Checks that e-mail address is valid.
        /// </summary>
        public ValidEmailAttribute() : base(DataType.EmailAddress)
        {
        }

        /// <inheritdoc />
        public override bool IsValid(object? value)
        {
            if (value == null) return true; // attribute [Required] should checks value for null

            return EmailHelper.IsEmailValid(value as string);
        }

        /// <inheritdoc />
        public override string FormatErrorMessage(string name)
        {
            return ErrorMessage;
        }
    }
}