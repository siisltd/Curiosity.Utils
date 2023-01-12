using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace Curiosity.Tools.Web.Cookies
{
    /// <summary>
    /// Solving problems with SameSite in cookies (fixing authorization in WebData, working in iframe in old browsers)
    /// </summary>
    /// <remarks>
    /// https://devblogs.microsoft.com/aspnet/upcoming-samesite-cookie-changes-in-asp-net-and-asp-net-core/
    /// </remarks>
    public static class CookieSameSiteIssueResolver
    {
        /// <summary>
        /// Unspecified appeared in .NET Core 3.1, for versions .NET Core below 3.1 is advised to use = -1
        /// </summary>
        private const SameSiteMode Unspecified = SameSiteMode.Unspecified;

        /// <summary>
        /// Configures cookies so that they work in older browsers (and so that the iframe works)
        /// </summary>
        /// <param name="services"></param>
        public static void ConfigureCookiesForOldBrowsers(this IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
                options.Secure = CookieSecurePolicy.SameAsRequest;
                options.MinimumSameSitePolicy = Unspecified;
                options.OnAppendCookie = cookieContext => 
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
                options.OnDeleteCookie = cookieContext => 
                    CheckSameSite(cookieContext.Context, cookieContext.CookieOptions);
            });
        }
        
        /// <summary>
        /// Checks whether to set SameSite.Unspecified and exposes, if necessary
        /// </summary>
        private static void CheckSameSite(HttpContext httpContext, CookieOptions options)
        {
            if (options.SameSite > Unspecified)
            {
                var userAgent = httpContext.Request.Headers["User-Agent"].ToString();
                if (DisallowsSameSiteNone(userAgent))
                {
                    options.SameSite = Unspecified;
                }
            }
        }
        
        /// <summary>
        /// Checks whether the user agent was included in the list of those that do not support SameSite.None and for which you need to set Unspecified
        /// </summary>
        private static bool DisallowsSameSiteNone(string userAgent)
        {
            // Cover all iOS based browsers here. This includes:
            // - Safari on iOS 12 for iPhone, iPod Touch, iPad
            // - WkWebview on iOS 12 for iPhone, iPod Touch, iPad
            // - Chrome on iOS 12 for iPhone, iPod Touch, iPad
            // All of which are broken by SameSite=None, because they use the iOS networking stack
            if (userAgent.Contains("CPU iPhone OS 12") || userAgent.Contains("iPad; CPU OS 12"))
            {
                return true;
            }

            // Cover Mac OS X based browsers that use the Mac OS networking stack. This includes:
            // - Safari on Mac OS X.
            // This does not include:
            // - Chrome on Mac OS X
            // Because they do not use the Mac OS networking stack.
            if (userAgent.Contains("Macintosh; Intel Mac OS X 10_14") && 
                userAgent.Contains("Version/") && userAgent.Contains("Safari"))
            {
                return true;
            }

            // Cover Chrome 50-69, because some versions are broken by SameSite=None, 
            // and none in this range require it.
            // Note: this covers some pre-Chromium Edge versions, 
            // but pre-Chromium Edge does not require SameSite=None.
            if (userAgent.Contains("Chrome/5") || userAgent.Contains("Chrome/6"))
            {
                return true;
            }

            return false;
        }
    }
}