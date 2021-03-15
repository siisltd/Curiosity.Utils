using System;
using System.Threading.Tasks;
using Curiosity.Configuration;
using Microsoft.AspNetCore.Http;

namespace Curiosity.Tools.Web.Middleware
{
    /// <summary>
    /// Middleware for detecting requests from bots/services that make preview pages.
    /// </summary>
    public class PreviewDetectMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly PreviewDetectOptions _options;

        public PreviewDetectMiddleware(
            RequestDelegate next,
            PreviewDetectOptions options)
        {
            if (options == null) throw new ArgumentNullException(nameof(options));
            options.AssertValid();
            
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _options = options;
        }

        public Task Invoke(HttpContext context)
        {
            // if this is a bot that makes a Preview, then we will redirect to the specified page
            if (context.IsPreviewRequest())
            {
                context.Request.Path = _options.RedirectPath;
            }
            
            return _next(context);
        }
    }
}