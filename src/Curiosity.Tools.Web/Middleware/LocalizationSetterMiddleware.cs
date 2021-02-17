using System;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.Tools.Web.Cookies;
using Microsoft.AspNetCore.Http;

namespace Curiosity.Tools.Web.Middleware
{
    /// <summary>
    /// Middleware for setting the locale in cookies.
    /// </summary>
    /// <remarks>
    /// Standard middleware only puts the culture in the current stream, but does not save it between requests. This middleware just saves it.
    /// </remarks>
    public class LocalizationSetterMiddleware
    {
        private readonly RequestDelegate _next;

        public LocalizationSetterMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }

        public Task InvokeAsync(HttpContext context)
        {
            // the locale can also be updated via the URL, but then you also need to pass the parameter in AJAX requests, this is a pain in the ass
            // // that's why we always set cookies
            context.Response.UpdateLanguageCookie(Thread.CurrentThread.CurrentCulture.TwoLetterISOLanguageName);
            
            return _next(context);
        }
    }
}