using System;
using System.Runtime.ExceptionServices;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Server.Kestrel.Core;
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

        private readonly RequestDelegate _exceptionHandler;
        
        public CuriosityExceptionHandleMiddleware(
            RequestDelegate next,
            ILogger<CuriosityExceptionHandleMiddleware> logger,
            IOptions<CuriosityExceptionHandlerOptions> options)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options?.Value ?? throw new ArgumentNullException(nameof(options));
            
            _clearCacheHeadersDelegate = ClearCacheHeaders;
            
            if (_options.ExceptionHandlingPath == null)
                throw new InvalidOperationException($"{nameof(_options.ExceptionHandlingPath)} can't be empty.");

            _exceptionHandler = _next;
        }

        public Task Invoke(HttpContext context)
        {
            ExceptionDispatchInfo edi;
            try
            {
                var task = _next(context);
                if (!task.IsCompletedSuccessfully)
                {
                    return Awaited(this, context, task);
                }

                return Task.CompletedTask;
            }
            catch (Exception exception)
            {
                // Get the Exception, but don't continue processing in the catch block as its bad for stack usage.
                edi = ExceptionDispatchInfo.Capture(exception);
            }

            return HandleException(context, edi);

            static async Task Awaited(CuriosityExceptionHandleMiddleware middleware, HttpContext context, Task task)
            {
                ExceptionDispatchInfo edi = null;
                try
                {
                    await task;
                }
                catch (Exception exception)
                {
                    // Get the Exception, but don't continue processing in the catch block as its bad for stack usage.
                    edi = ExceptionDispatchInfo.Capture(exception);
                }

                if (edi != null)
                {
                    await middleware.HandleException(context, edi);
                }
            }
        }

        private async Task HandleException(HttpContext context, ExceptionDispatchInfo edi)
        {
            // пишем в warning, потому что у нас в проектах в другом месте месте определяется, что идет в error, а что в info
            _logger.LogWarning(edi.SourceException, $"An unhandled exception has occurred while executing the request (Path: \"{context.Request.Path}\"; Query: \"{context.Request.QueryString}\").");
            
            // We can't do anything if the response has already started, just abort.
            if (context.Response.HasStarted)
            {
                _logger.LogWarning($"Can't handle error for request {context.Request.Path} because, The response has already started, the error handler will not be executed.");
                edi.Throw();
            }

            var originalPath = context.Request.Path;
            if (_options.ExceptionHandlingPath.HasValue)
            {
                context.Request.Path = _options.ExceptionHandlingPath;
            }
            try
            {
                ClearHttpContext(context);

                var exceptionHandlerFeature = new ExceptionHandlerFeature()
                {
                    Error = edi.SourceException,
                    Path = originalPath.Value,
                };
                context.Features.Set<IExceptionHandlerFeature>(exceptionHandlerFeature);
                context.Features.Set<IExceptionHandlerPathFeature>(exceptionHandlerFeature);
                context.Response.StatusCode = 500;
                context.Response.OnStarting(_clearCacheHeadersDelegate, context.Response);

                await _exceptionHandler(context);

                // TODO: Optional re-throw? We'll re-throw the original exception by default if the error handler throws.
                return;
            }
            catch (BadHttpRequestException ex2)
            {
                // https://github.com/dotnet/aspnetcore/issues/23949 ошибка возникает, когда пользователя отменяет запрос.
                // Тогда в своей обработки мы не можем обновить response. MS тупо советуют не реагировать на ошибку.
                _logger.LogWarning(ex2, $"Request was cancelled by user. So we can't update the response. \"{context.Request.Path}\".");

                return;
            }
            catch (Exception ex2)
            {
                // Suppress secondary exceptions, re-throw the original.
                _logger.LogError(ex2, "An exception was thrown attempting to execute the error handler.");
            }
            finally
            {
                context.Request.Path = originalPath;
            }

            edi.Throw(); // Re-throw the original if we couldn't handle it
        }

        private static void ClearHttpContext(HttpContext context)
        {
            context.Response.Clear();

            // An endpoint may have already been set. Since we're going to re-invoke the middleware pipeline we need to reset
            // the endpoint and route values to ensure things are re-calculated.
            context.SetEndpoint(endpoint: null);
            var routeValuesFeature = context.Features.Get<IRouteValuesFeature>();
            routeValuesFeature?.RouteValues?.Clear();
        }

        private static Task ClearCacheHeaders(object state)
        {
            var headers = ((HttpResponse)state).Headers;
            headers[HeaderNames.CacheControl] = "no-cache";
            headers[HeaderNames.Pragma] = "no-cache";
            headers[HeaderNames.Expires] = "-1";
            headers.Remove(HeaderNames.ETag);
            return Task.CompletedTask;
        }
    }
}