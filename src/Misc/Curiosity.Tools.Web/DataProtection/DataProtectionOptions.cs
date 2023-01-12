using System;
using System.Collections.Generic;
using System.IO;
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
        /// The path to the directory with the key ring files.
        /// </summary>
        public string? KeyRingPath { get; set; }
        
        /// <inheritdoc/>
        public IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
        {
            var errors = new ConfigurationValidationErrorCollection(prefix);

            if (IsEnabled)
            {
                errors.AddErrorIf(String.IsNullOrWhiteSpace(ApplicationName), nameof(ApplicationName), "Can't be empty or null");

                if (String.IsNullOrWhiteSpace(KeyRingPath))
                {
                    errors.AddError(nameof(KeyRingPath), "Can't be empty or null");
                }
                else
                {
                    if (!Directory.Exists(KeyRingPath))
                    {
                        errors.AddError(nameof(KeyRingPath), "Specified directory doesn't exist");
                    }
                }
            }
            
            return errors;
        }
    }
}