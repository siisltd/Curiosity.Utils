using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Curiosity.Tools.Web.ReverseProxy
{
    /// <summary>
    /// Extension methods for <see cref= "IApplicationBuilder"/>, which include support for running the server behind a proxy
    /// </summary>
    public static class ApplicationBuilderProxyExtensions
    {
        /// <summary>
        /// Enables support for the application to work behind the reverse proxy.
        /// If <see cref= "configuration" /> <see langword= "null"/>, there will be no support
        /// </summary>
        /// <remarks>
        /// Must call before enabling authorization and other handlers
        /// </remarks>
        /// <param name="app"></param>
        /// <param name= "configuration" >reverse proxy Settings</param>
        /// <returns></returns>
        /// <exception cref= "ArgumentNullException" >If <see cref= " app "/> <see langowrd= "null" /></exception>
        public static IApplicationBuilder UseReverseProxy(
            this IApplicationBuilder app,
            ReverseProxyOptions? configuration = null)
        {
            if (app == null) throw new ArgumentNullException(nameof(app));
            if (configuration == null) return app;
            
            app.UseForwardedHeaders();

            if (!string.IsNullOrWhiteSpace(configuration.PathBase))
            {
                app.UsePathBase(configuration.PathBase);
                app.Use((context, next) =>
                {
                    context.Request.PathBase = new PathString(configuration.PathBase);
                    return next();
                });
            }

            return app;
        }
    }
}