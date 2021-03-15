using System;
using Curiosity.Configuration;
using Microsoft.AspNetCore.Builder;

namespace Curiosity.Tools.Web.Middleware
{
    public static partial class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds middleware that detects requests from preview bots/services and redirects to the specified page.
        /// </summary>
        public static IApplicationBuilder UsePreviewDetector(this IApplicationBuilder app, string redirectPath)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (String.IsNullOrWhiteSpace(redirectPath))
            {
                throw new ArgumentNullException(nameof(redirectPath));
            }

            return app.UsePreviewDetector(new PreviewDetectOptions
            {
                RedirectPath = redirectPath
            });
        }
        
        /// <summary>
        /// Adds middleware that detects requests from preview bots/services and redirects to the specified page.
        /// </summary>
        public static IApplicationBuilder UsePreviewDetector(this IApplicationBuilder app, PreviewDetectOptions options)
        {
            if (app == null)
            {
                throw new ArgumentNullException(nameof(app));
            }
            if (options == null)
            {
                throw new ArgumentNullException(nameof(options));
            }
            options.AssertValid();

            return app.UseMiddleware<PreviewDetectMiddleware>(options);
        }
    }
}