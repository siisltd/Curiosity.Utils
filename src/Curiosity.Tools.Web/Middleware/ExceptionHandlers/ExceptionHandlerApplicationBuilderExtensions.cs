using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;

namespace Curiosity.Tools.Web.Middleware
{
    public static class ExceptionHandlerApplicationBuilderExtensions
    {
        /// <summary>
        /// Adds a middleware to the pipeline that will catch exceptions, and re-execute the request in an alternate pipeline.
        /// The request will not be re-executed if the response has already started.
        /// The request will not be logged.
        /// </summary>
        public static IApplicationBuilder UseCuriosityExceptionHandler(this IApplicationBuilder app, string errorHandlingPath)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));
            if (String.IsNullOrWhiteSpace(errorHandlingPath))
                throw new ArgumentNullException(nameof(errorHandlingPath));

            return app.UseCuriosityExceptionHandler(new CuriosityExceptionHandlerOptions
            {
                ExceptionHandlingPath = errorHandlingPath
            });
        }
        
        /// <summary>
        /// Adds a middleware to the pipeline that will catch exceptions, and re-execute the request in an alternate pipeline.
        /// The request will not be re-executed if the response has already started.
        /// The request will not be logged.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="options"></param>
        /// <returns></returns>
        public static IApplicationBuilder UseCuriosityExceptionHandler(this IApplicationBuilder app, CuriosityExceptionHandlerOptions options)
        {
            if (app == null)
                throw new ArgumentNullException(nameof(app));
            if (options == null)
                throw new ArgumentNullException(nameof(options));

            return app.UseMiddleware<CuriosityExceptionHandleMiddleware>(Options.Create(options));
        }
    }
}