using System.Collections.Generic;
using Curiosity.Configuration;

namespace Curiosity.Hosting.Web
{
    /// <summary>
    /// Kestrel options.
    /// </summary>
    /// <remarks>
    /// The Kestrel settings themselves are hardly used in our code. Class are made to:
    /// - output config values to the log
    /// - perform validation
    /// Only Endpoints are placed in this class, because we use them when configuring Cors and when check the existence of the certificate.
    /// Other parameters can be simply added to the config.
    /// </remarks>
    public class KestrelOptions : ILoggableOptions, IValidatableOptions
    {
        /// <summary>
        /// Endpoints that Kestrel will listen to and use to access the app
        /// </summary>
        public Dictionary<string, EndpointOptions> Endpoints { get; set; } = new Dictionary<string, EndpointOptions>();

        /// <inheritdoc />
        public IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
        {
            var errors = new ConfigurationValidationErrorCollection(prefix);

            // ReSharper disable once ConditionIsAlwaysTrueOrFalse
            if (Endpoints != null && Endpoints.Count > 0)
            {
                foreach (var kvp in Endpoints)
                {
                    var endpointName = kvp.Key;
                    var config = kvp.Value;

                    if (config == null)
                    {
                        errors.AddError(prefix + endpointName, "Can't be empty");
                    }
                    else
                    {
                        errors.AddErrors(config.Validate(prefix + $":{endpointName}"));
                    }
                }
            }

            return errors;
        }
    }
}