using System;
using Curiosity.Tools.Web.Localization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;

namespace Curiosity.Tools.Web.Cookies
{
    /// <summary>
    /// Assistant in localization processing via cookies
    /// </summary>
    public static class LocalizationCookieHelper
    {
        /// <summary>
        /// Updates information about the current localization in the ASP.NET cookie
        /// </summary>
        public static string UpdateLanguageCookie(this HttpResponse response, string? language)
        {
            if (response == null) throw new ArgumentNullException(nameof(response));
            
            // if you didn't specify a locale or we don't know it, we'll use the default value.
            if (String.IsNullOrEmpty(language) || !LngHelper.AvailableLanguages.ContainsKey(language))
            {
                language = LngHelper.DefaultLanguage;
            }
            
            // update ASP.NET Core cookies for localization
            response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(language)),
                new CookieOptions { 
                    Expires = DateTimeOffset.UtcNow.AddYears(1), 
                    // crutch for embedding in an iframe
                    SameSite = SameSiteMode.None,
                    Secure = true
                }
            );

            return language;
        } 
    }
}