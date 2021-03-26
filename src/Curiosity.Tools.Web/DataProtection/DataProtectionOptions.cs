using System;
using System.Collections.Generic;
using Curiosity.Configuration;

namespace Curiosity.Tools.Web.DataProtection
{
    /// <summary>
    /// Options for configuring DataProtection in Curiosity ASP.NET Core applications.
    /// </summary>
    public class DataProtectionOptions : IValidatableOptions, ILoggableOptions
    {
        /// <summary>
        /// Is DataProtection protection enabled (ASP.NET Core)
        /// </summary>
        public bool IsEnabled { get; set; } = false;
        
        /// <summary>
        /// The name of the application. Used for encryption.
        /// </summary>
        public string? ApplicationName { get; set; }
        
        /// <summary>
        /// The path to the file with the key ring.
        /// </summary>
        public string? KeyRingPath { get; set; }
        
        /// <inheritdoc/>
        public IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
        {
            var errors = new ConfigurationValidationErrorCollection(prefix);

            if (IsEnabled)
            {
                errors.AddErrorIf(String.IsNullOrWhiteSpace(ApplicationName), nameof(ApplicationName), "Can't be empty or null");
                errors.AddErrorIf(String.IsNullOrWhiteSpace(KeyRingPath), nameof(KeyRingPath), "Can't be empty or null");
            }
            
            return errors;
        }
    }
}