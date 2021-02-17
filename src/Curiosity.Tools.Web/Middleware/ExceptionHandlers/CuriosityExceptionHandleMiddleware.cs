using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;

namespace Curiosity.Tools.Web.Middleware
{
    /// <summary>
    /// Middleware to handle exception at web applications.
    /// </summary>
    public class CuriosityExceptionHandleMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly CuriosityExceptionHandlerOptions _options;
        private readonly ILogger _logger;
        private readonly Func<object, Task> _clearCacheHeadersDelegate;

        public CuriosityExceptionHandleMiddleware(
            RequestDelegate next,
            ILogger<CuriosityExceptionHandleMiddleware> logger,
            IOptions<CuriosityExceptionHandlerOptions> options)
        {
            _next = next;
            _options = options.Value;
            _logger = logger;
            _clearCacheHeadersDelegate = ClearCacheHeaders;
            
            if (_options.ExceptionHandlingPath == null)
            {
                throw new InvalidOperationException($"{nameof(_options.ExceptionHandlingPath)} can't be empty.");
            }
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // We can't do anything if the response has already started, just abort.
                if (context.Response.HasStarted)
                {
                    _logger.LogError(ex, $"Can't handle error for request {context.Request.Path} because request has been already started.");
                    throw;
                }

                PathString originalPath = context.Request.Path;
                if (_options.ExceptionHandlingPath.HasValue)
                {
                    context.Request.Path = _options.ExceptionHandlingPath;
                }
                try
                {
                    context.Response.Clear();
                    var exceptionHandlerFeature = new ExceptionHandlerFeature()
                    {
                        Error = ex,
                        Path = originalPath.Value,
                    };
                    context.Features.Set<IExceptionHandlerFeature>(exceptionHandlerFeature);
                    context.Features.Set<IExceptionHandlerPathFeature>(exceptionHandlerFeature);
                    context.Response.StatusCode = 500;
                    context.Response.OnStarting(_clearCacheHeadersDelegate, context.Response);

                    await _next(context);

                    return;
                }
                catch (Exception ex2)
                {
                    // Suppress secondary exceptions, re-throw the original.
                    _logger.LogError(ex2, $"Error occured while handling error for request \"{context.Request.Path}\".");
                }
                finally
                {
                    context.Request.Path = originalPath;
                }
                throw; // Re-throw the original if we couldn't handle it
            }
        }

        private Task ClearCacheHeaders(object state)
        {
            var response = (HttpResponse)state;
            response.Headers[HeaderNames.CacheControl] = "no-cache";
            response.Headers[HeaderNames.Pragma] = "no-cache";
            response.Headers[HeaderNames.Expires] = "-1";
            response.Headers.Remove(HeaderNames.ETag);
            return Task.CompletedTask;
        }
    }
}