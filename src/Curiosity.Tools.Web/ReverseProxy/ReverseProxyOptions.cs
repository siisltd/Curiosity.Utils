using System;
using System.Collections.Generic;
using Curiosity.Configuration;

namespace Curiosity.Tools.Web.ReverseProxy
{
    /// <summary>
    /// Reverse proxy options that may affect the operation of our server
    /// </summary>
    /// <remarks>
    /// Details about working for a proxy on https://docs.microsoft.com/ru-ru/aspnet/core/host-and-deploy/proxy-load-balancer?view=aspnetcore-2.2
    /// </remarks>
    public class ReverseProxyOptions : ILoggableOptions, IValidatableOptions
    {
        /// <резюме>
        /// Address of the server behind which the proxy is hidden
        /// </резюме>
        /// <пример>
        /// example.domain.me/api
        /// </пример>
        public string PathBase { get; set; } = null!;

        /// <summary>
        /// IP reverse proxy
        /// </summary>
        /// <remarks>
        /// Needed to add the ip to the white list, otherwise the server will ignore requests
        /// By default, the whitelist is 127.0.0.1
        /// </remarks>
        public string[]? ProxyIps { get; set; }

        /// <inheritdoc />
        public virtual IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
        {
            var errors = new ConfigurationValidationErrorCollection(prefix);

            errors.AddErrorIf(String.IsNullOrWhiteSpace(PathBase), nameof(PathBase), "can't be empty");
            
            return errors;
        }
    }
}