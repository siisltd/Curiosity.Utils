using System;
using System.Collections.Generic;
using Curiosity.Configuration;

namespace Curiosity.Tools.Web.ReCaptcha
{
    /// <summary>
    /// Options required for the reCAPTCHA service to work.
    /// </summary>
    public class ReCaptchaOptions : IValidatableOptions, ILoggableOptions
    { 
        /// <summary>
        /// Address of the reCAPTCHA API.
        /// </summary>
        public string ReCaptchaApiUrl { get; set; } = null!;
        
        /// <summary>
        /// Use this key in the HTML code that your site transmits to users ' devices.
        /// </summary>
        public string ReCaptchaClientKey { get; set; } = null!;
        
        /// <summary>
        /// Use this secret key to exchange data between the site and the reCAPTCHA service.
        /// </summary>
        public string ReCaptchaServerKey { get; set; } = null!;
        
        /// <inheritdoc/>
        public int RetryTimeoutSec { get; set; } = 1;

        /// <inheritdoc/>
        public int RetryCount { get; set; } = 3;

        /// <inheritdoc />
        public virtual IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
        {
            var errors = new ConfigurationValidationErrorCollection(prefix);

            errors.AddErrorIf(String.IsNullOrWhiteSpace(ReCaptchaApiUrl), nameof(ReCaptchaApiUrl), "can not be empty");
            errors.AddErrorIf(String.IsNullOrWhiteSpace(ReCaptchaClientKey), nameof(ReCaptchaClientKey), "can not be empty");
            errors.AddErrorIf(String.IsNullOrWhiteSpace(ReCaptchaServerKey), nameof(ReCaptchaServerKey), "can not be empty");
            errors.AddErrorIf(RetryTimeoutSec < 0, nameof(RetryTimeoutSec), "should be greater than 0");
            errors.AddErrorIf(RetryCount < 0, nameof(RetryCount), "should be greater than 0");
            
            return errors;
        }
    }
}