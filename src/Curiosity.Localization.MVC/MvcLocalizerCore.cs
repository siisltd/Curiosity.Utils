using System;
using System.Collections.Concurrent;
using System.Reflection;
using System.Threading;
using Microsoft.AspNetCore.Mvc.Localization;

namespace Curiosity.Localization.MVC
{
    public class MvcLocalizerCore : LocalizerCore
    {
        public MvcLocalizerCore(Assembly assembly, LocalizationOptions options) : base(assembly, options)
        {
        }

        /// <summary>
        /// Retrieving a string containing HTML from resources corresponding to the UI culture of the current thread.
        /// </summary>
        /// <param name="prefix">Key prefix.</param>
        /// <param name="source">The key for which we are looking for localization.</param>
        /// <param name="arguments">Substitution arguments.</param>
        public LocalizedHtmlString GetHtml(string prefix, string source, params object[] arguments)
        {
            var currentLanguage = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
            if (!String.IsNullOrEmpty(source))
            {
                var resourceKey = ResourceKeyGenerator.Generate(prefix, source);
                var result = CachedStrings
                    .GetOrAdd(
                        currentLanguage,
                        (lang) => new ConcurrentDictionary<string, string>())
                    .GetOrAdd(
                        resourceKey,
                        (key) => ResourceManager.GetString(key) ?? source);
                return new LocalizedHtmlString(resourceKey, result, false, arguments);
            }

            return new LocalizedHtmlString(String.Empty, String.Empty);
        }
    }
}
