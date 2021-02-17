using System;
using System.Collections.Generic;
using System.IO;
using Curiosity.Configuration;
using Curiosity.Tools;

namespace Curiosity.Hosting.Web
{
    /// <summary>
    /// Some Kestrel's endpoint options.
    /// </summary>
    public class EndpointOptions : ILoggableOptions, IValidatableOptions
    {
        /// <summary>
        /// Url to listen.
        /// </summary>
        public string Url { get; set; } = null!;

        /// <summary>
        /// SSL certificate.
        /// </summary>
        public EndpointCertificate? Certificate { get; set; }

        /// <inheritdoc />
        public IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
        {
            var errors = new ConfigurationValidationErrorCollection(prefix);
            
            if (String.IsNullOrEmpty(Url))
            {
                errors.AddError(nameof(Url), "can't be empty");
            }
            else if (!WebAddressValidator.IsValid(Url))
            {
                errors.AddError(nameof(Url), $"\"{Url}\" - isn't correct url");
            }
            
            if (Certificate != null)
            {
                errors.AddErrors(Certificate.Validate(prefix + $":{nameof(Certificate)}"));
            }

            return errors;
        }
        
        /// <summary>
        /// SSL certificate for specific endpoint.
        /// </summary>
        public class EndpointCertificate : ILoggableOptions, IValidatableOptions
        {
            /// <summary>
            /// Path to SSL certificate (*.pfx file).
            /// </summary>
            public string Path { get; set; } = null!;

            /// <summary>
            /// Password for certificate.
            /// </summary>
            public string? Password { get; set; }

            /// <inheritdoc />
            public IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
            {
                var errors = new ConfigurationValidationErrorCollection(prefix);
                if (String.IsNullOrWhiteSpace(Path))
                {
                    errors.AddError(nameof(Path), "Can't be empty");
                }
                else if (!File.Exists(Path))
                {
                    errors.AddError(nameof(Path), $"Certificate not found (Path = \"{Path}\")");
                }

                return errors;
            }
        }
    }
}