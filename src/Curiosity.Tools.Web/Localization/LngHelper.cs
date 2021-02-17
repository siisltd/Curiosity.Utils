using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
using System.Threading;

namespace Curiosity.Tools.Web.Localization
{
    public static class LngHelper
    {
        static LngHelper()
        {
            var availableLanguages = new Dictionary<string, Tuple<string, CultureInfo>>();

            var enCultureInfo = new CultureInfo("en")
            {
                DateTimeFormat = {ShortDatePattern = "MM/dd/yyyy"}
            };

            var ruCultureInfo = new CultureInfo("ru")
            {
                DateTimeFormat = {ShortDatePattern = "dd.MM.yyyy"}
            };

            availableLanguages.Add("en", new Tuple<string, CultureInfo>("English", enCultureInfo));
            availableLanguages.Add("ru", new Tuple<string, CultureInfo>("Русский", ruCultureInfo));

            AvailableLanguages = availableLanguages;
        }

        public static IReadOnlyDictionary<string, Tuple<string, CultureInfo>> AvailableLanguages { get; }
        
        public static CultureInfo CurrentCulture => Thread.CurrentThread.CurrentUICulture;
        
        public static string CurrentLanguage => Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower();

        public static string DefaultLanguage => "ru";
        
        public static bool IsCurrentLangRu()
        {
            return "ru".Equals(CurrentLanguage, StringComparison.OrdinalIgnoreCase);
        }
        public static bool IsCurrentLangRu(string language)
        {
            return "ru".Equals(language, StringComparison.OrdinalIgnoreCase);
        }
        
        public static bool IsCurrentLangEn()
        {
            return "en".Equals(CurrentLanguage, StringComparison.OrdinalIgnoreCase);
        }
        public static bool IsCurrentLangEn(string language)
        {
            return "en".Equals(language, StringComparison.OrdinalIgnoreCase);
        }
        
        public static bool IsCurrentLangUnknown()
        {
            return !AvailableLanguages.ContainsKey(CurrentLanguage);
        }
        
        /// <summary>
        /// Updates the value of the culture parameter in the request to the specified language
        /// </summary>
        public static string UpdateCultureInUrl(string url, string language)
        {
            // if the return address is not empty, we will update the culture in the url, if there is one
            if (!String.IsNullOrWhiteSpace(url))
            {
                // we have 2-3 characters per locale.
                var regex = new Regex(@"culture=(\w){2,3}", RegexOptions.IgnoreCase);
                
                // delete something dumb from the url, you never know, the user in the questionnaire counts on this parameter
                // // so replace the culture with the specified one
                url = regex.Replace(url, $"culture={language}");
            }

            return url;
        }
    }
}