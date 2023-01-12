using System;
using System.Net;
using Curiosity.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.DependencyInjection;

namespace Curiosity.Tools.Web.ReverseProxy
{
    /// <summary>
    /// Extension methods for <see cref= "IServiceCollection" /> that include support for the server running behind a proxy
    /// </summary>
    public static class IoCExtensions
    {
        /// <summary>
        /// Configures support for the application for the reverse proxy.
        /// If <see cref= "options" /> <see langword= "null"/>, there will be no support
        /// </summary>
        /// <remarks>
        /// Must call before enabling authorization and other handlers
        /// </remarks>
        /// <param name="services"></param>
        /// <param name= "options">Reverse proxy options</param>
        /// <returns></returns>
        /// <exception cref= "ArgumentNullException" >If <see cref= "services" /> <see langowrd= "null" /></exception>
        public static IServiceCollection AddReverseProxy(
            this IServiceCollection services,
            ReverseProxyOptions? options = null)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (options == null) return services;
            
            options.AssertValid();

            services.Configure<ForwardedHeadersOptions>(configureOptions =>
            {
                if (options.ProxyIps != null)
                {
                    foreach (var proxyIp in options.ProxyIps)
                    {
                        if (String.IsNullOrWhiteSpace(proxyIp)) continue;
                        
                        configureOptions.KnownProxies.Add(IPAddress.Parse(proxyIp));
                    }
                }
                
                configureOptions.ForwardedHeaders = 
                    ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
            });
            
            return services;
        }
    }
}