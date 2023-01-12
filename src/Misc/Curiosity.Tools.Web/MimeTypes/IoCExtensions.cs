using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.StaticFiles;
using Microsoft.Extensions.DependencyInjection;

namespace Curiosity.Tools.Web.MimeTypes
{
    /// <summary>
    /// Extension methods for <see cref= "IServiceCollection"/>
    /// </summary>
    public static class IoCExtensions
    {
        /// <summary>
        /// Registers the service to get the Mime type by file name
        /// </summary>
        /// <param name="services">IoC</param>
        /// <param name="additionalMapping">Additional values for mapping</param>
        /// <returns></returns>
        public static IServiceCollection AddMimeTypeMapping(
            this  IServiceCollection services,
            IDictionary<string, string>? additionalMapping = null)
        {
            var provider = new FileExtensionContentTypeProvider();
            if (additionalMapping != null)
            {
                foreach (var kvp in additionalMapping)
                {
                    if (String.IsNullOrWhiteSpace(kvp.Key) || String.IsNullOrWhiteSpace(kvp.Value)) continue;
                    
                    provider.Mappings.Add(kvp.Key, kvp.Value);
                }
            }
            services.AddSingleton(new MimeMappingService(provider));

            return services;
        }
    }
}