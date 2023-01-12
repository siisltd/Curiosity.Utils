using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Curiosity.Tools.Web.Middleware
{
    /// <summary>
    /// Middleware for creating unique id for each user without authenticating.
    /// </summary>
    //todo #5
    public class UserTraceMiddleware
    {
        private const string UserTraceCookieName = "curiosity-user-trace-id";
        
        private readonly RequestDelegate _next;

        public UserTraceMiddleware(RequestDelegate next)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
        }
        
        public async Task InvokeAsync(HttpContext context)
        {
            // try to fetch user trace id from cookies
            // if no such cookie, create a new one
            if (!context.Request.Cookies.TryGetValue(UserTraceCookieName, out var userTraceIdCookieValue) || !PublicId.TryParse(userTraceIdCookieValue, out var temp))
            {
                long? userTraceId = UniqueIdGenerator.Generate();

                context.Response.Cookies.Append(
                    UserTraceCookieName,
                    userTraceId.Value.ToPublicId(),
                    new CookieOptions
                    {
                        SameSite = SameSiteMode.None,
                        IsEssential = true,
                        MaxAge = TimeSpan.FromDays(365)
                    });
            }

            await _next(context);
        }
    }
}