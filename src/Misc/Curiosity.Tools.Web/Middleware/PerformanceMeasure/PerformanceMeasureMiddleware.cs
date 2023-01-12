using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Curiosity.Tools.Web.Middleware
{
    public class PerformanceMeasureMiddleware 
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;

        public PerformanceMeasureMiddleware(
            RequestDelegate next,
            ILoggerFactory loggerFactory)
        {
            if (loggerFactory == null) throw new ArgumentNullException(nameof(loggerFactory));
            
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = loggerFactory.CreateLogger("performanceWeb");
        }
        
        public async Task InvokeAsync(HttpContext context)
        {
            long startTicks = 0;
            var noErrors = false;
            // the url in the app may change after processing,
            // for example, show a page with errors without a redirect
            // for debugging, it is better to have both URLs
            var urlBeforeExecution = context.Request.Path;
            try
            {
                startTicks = DateTime.UtcNow.Ticks;
                await _next(context);
                noErrors = true;
            }
            finally
            {
                var endTicks = DateTime.UtcNow.Ticks;
                _logger.LogInformation(
                    $"IP: {context.Connection.RemoteIpAddress}; " +
                    $"URL before: \"{urlBeforeExecution}\"; " +
                    $"URL after: \"{context.Request.Path}\"; " +
                    $"time: {TimeSpan.FromTicks(endTicks - startTicks).TotalMilliseconds} ms; " +
                    $"status code: {(noErrors ? context.Response.StatusCode : 500)}; " +
                    $"query: \"{context.Request.QueryString}\"");
            }
        }
    }
}