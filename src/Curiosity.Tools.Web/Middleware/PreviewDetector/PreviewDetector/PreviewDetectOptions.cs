using System;
using System.Collections.Generic;
using Curiosity.Configuration;
using Microsoft.AspNetCore.Http;

namespace Curiosity.Tools.Web.Middleware.PreviewDetector.PreviewDetector
{
    /// <summary>
    /// Options for the <see cref="PreviewDetectMiddleware"/>.
    /// </summary>
    public class PreviewDetectOptions : IValidatableOptions, ILoggableOptions
    {
        /// <summary>
        /// The path to redirect to if a Preview request is detected
        /// </summary>
        public PathString RedirectPath { get; set; }

        public IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
        {
            var errors = new ConfigurationValidationErrorCollection(prefix);
            
            errors.AddErrorIf(String.IsNullOrWhiteSpace(RedirectPath), nameof(RedirectPath), "can't be empty");

            return errors;
        }
    }
}