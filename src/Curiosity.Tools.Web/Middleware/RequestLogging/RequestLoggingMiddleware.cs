using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace Curiosity.Tools.Web.Middleware
{
    /// <summary>
    /// Middleware that logs all incoming requests
    /// </summary>
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger _logger;
        private readonly SensitiveDataProtector _dataProtector;

        public RequestLoggingMiddleware(
            RequestDelegate next, 
            ILogger logger, 
            IServiceProvider serviceProvider)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            
            _dataProtector = serviceProvider.GetService(typeof(SensitiveDataProtector)) as SensitiveDataProtector ?? new SensitiveDataProtector();
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var request = context.Request;

            if (request.Body != null)
            {
                var ip = context.Connection.RemoteIpAddress;
                if (request.IsMultipartContentType())
                {
                    _logger.LogDebug($"[REQUEST FROM {ip}] {request.Path}/{request.QueryString}: is mime multipart content, dump skipped");
                }
                else
                {
                    context.Response.Headers.Add("Trace-Id", context.TraceIdentifier);
                    
                    context.Request.EnableBuffering();

                    var buffer = new byte[Convert.ToInt32(context.Request.ContentLength)];
                    await context.Request.Body.ReadAsync(buffer, 0, buffer.Length);
                    var requestBody = Encoding.UTF8.GetString(buffer);
                    context.Request.Body.Seek(0, SeekOrigin.Begin);

                    if (String.IsNullOrEmpty(requestBody))
                    {
                        _logger.LogDebug($"[REQUEST WITHOUT CONTENT FROM {ip}] {request.Path}/{request.QueryString}");
                    }
                    else
                    {
                        _logger.LogDebug($"[REQUEST CONTENT FROM {ip}] {request.Path}/{request.QueryString}");
                        _logger.LogDebug(_dataProtector.HideInJson(requestBody));
                        _logger.LogDebug($"[END] {request.Path}");
                    }
                }
            }

            await _next(context);
        }
    }
}