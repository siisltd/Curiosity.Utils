using System;
using System.Collections.Generic;
using Curiosity.Configuration;
using Curiosity.Tools.Web;

namespace Curiosity.Hosting.Web
{
    /// <summary>
    /// Validator of <see cref="ICuriosityWebAppConfiguration"/> implementations.
    /// </summary>
    public static class CuriosityWebAppConfigurationValidator
    {
        /// <summary>
        /// Validates implementation of <see cref="ICuriosityWebAppConfiguration"/>
        /// </summary>
        public static IReadOnlyCollection<ConfigurationValidationError> Validate<T>(this T configuration, string? prefix) where T: ICuriosityWebAppConfiguration
        {
            var errors = new ConfigurationValidationErrorCollection(prefix);

            if (String.IsNullOrWhiteSpace(configuration.Urls) && configuration.Kestrel == null)
            {
                errors.AddError(nameof(configuration.Urls), $"{nameof(configuration.Urls)} or {nameof(configuration.Kestrel)} must be specified");
            }
            else if (configuration.Urls != null && configuration.Kestrel != null)
            {
                errors.AddError(nameof(configuration.Urls), $"{nameof(configuration.Urls)} and {nameof(configuration.Kestrel)} can't be used at same time");
            }

            if (configuration.Kestrel != null)
            {
                errors.AddErrors(configuration.Kestrel.Validate(prefix+$":{nameof(configuration.Kestrel)}"));
            }
            
            errors.AddErrors(configuration.ThreadPool.Validate(prefix + $":{nameof(configuration.ThreadPool)}"));
            errors.AddErrors(CuriosityAppConfigurationValidator.Validate(configuration, prefix));

            // ReSharper disable once SuspiciousTypeConversion.Global
            if (configuration is IWebAppConfigurationWithPublicDomain config)
            {
                errors.AddErrorIf(String.IsNullOrWhiteSpace(config.PublicDomain), nameof(config.PublicDomain), "can't be null");
            }
            
            return errors;
        }
    }
}