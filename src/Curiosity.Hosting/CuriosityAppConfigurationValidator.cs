using System;
using System.Collections.Generic;
using Curiosity.Configuration;

namespace Curiosity.Hosting
{
    public static class CuriosityAppConfigurationValidator
    {
        /// <summary>
        /// Validates configuration.
        /// </summary>
        public static IReadOnlyCollection<ConfigurationValidationError> Validate(
            this ICuriosityAppConfiguration configuration,
            string? prefix = null)
        {
            var errors = new ConfigurationValidationErrorCollection(prefix);
            
            errors.AddErrorIf(String.IsNullOrWhiteSpace(configuration.AppName), nameof(configuration.AppName), "Can't be empty");
            errors.AddErrors(configuration.Culture.Validate(nameof(configuration.Culture)));
            errors.AddErrors(configuration.Log.Validate(nameof(configuration.Log)));

            return errors;
        }
    }
}