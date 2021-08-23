using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;
using Microsoft.AspNetCore.Mvc.Localization;

namespace Curiosity.Localization
{
    public class LocalizerCore
    {
        private string _defaultLanguage;
        private ConcurrentDictionary<string, ConcurrentDictionary<string, string>> _cachedStrings;
        private ResourceManager _resourceManager;

        public LocalizerCore(Assembly assembly, LocalizationOptions options)
        {
            var baseName = $"{assembly.GetName().Name}.{options.ResourcesFolder}.{options.ResourcesFileName}";

            _defaultLanguage = options.DefaultLanguage.ToLower();
            _cachedStrings = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();
            _resourceManager = new ResourceManager(baseName, assembly);

            // Check resources for all supported languages. For the default language resources not required.
            if (options.CheckResourceFiles)
            {
                // Check resources for the main language.
                var resourcesName = $"{baseName}.resources";
                if (assembly?.GetManifestResourceInfo(resourcesName) == null)
                    throw new Exception($"The resources \"{resourcesName}\" were not found in the assembly.");
                
                // Check resources for translations.
                foreach (var supportedLanguage in options.SupportedLanguages)
                {
                    var language = supportedLanguage.ToLower();
                    resourcesName = $"{baseName}.{language}.resources";
                    var satelliteAssembly = assembly.GetSatelliteAssembly(new CultureInfo(language));
                    if (satelliteAssembly?.GetManifestResourceInfo(resourcesName) == null)
                        throw new Exception($"The resources \"{resourcesName}\" were not found in the assembly.");
                }
            }
        }

        /// <summary>
        /// Retrieving a string from resources using the UI culture of the current thread.
        /// By default, does not read strings from resources for the default language.
        /// </summary>
        /// <param name="prefix">Key prefix.</param>
        /// <param name="source">The text to be translated or the key for which we are looking for localization.</param>
        /// <param name="forceReadResources">If false - resources for default culture will not be read. Instead, the string
        /// passed as source will be returned. If source is a key and not the desired text, true must be set.</param>
        /// <param name="arguments">Arguments to substitute in the string format.</param>
        public string Get(string prefix, string source, bool forceReadResources = false, params object[] arguments)
        {
            var currentLanguage = Thread.CurrentThread.CurrentUICulture.TwoLetterISOLanguageName.ToLower();
            if (!String.IsNullOrEmpty(source))
            {
                string result;
                if (!forceReadResources && _defaultLanguage.Equals(currentLanguage, StringComparison.OrdinalIgnoreCase))
                {
                    result = source;
                }
                else
                {
                    result = _cachedStrings
                        .GetOrAdd(
                            currentLanguage,
                            (lang) => new ConcurrentDictionary<string, string>())
                        .GetOrAdd(
                            ResourceKeyGenerator.Generate(prefix, source),
                            (key) => _resourceManager.GetString(key) ?? source);
                }

                if (arguments == null || arguments.Length == 0)
                    return result;
               
                return String.Format(result, arguments);
            }

            return String.Empty;
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
                var result = _cachedStrings
                    .GetOrAdd(
                        currentLanguage,
                        (lang) => new ConcurrentDictionary<string, string>())
                    .GetOrAdd(
                        resourceKey,
                        (key) => _resourceManager.GetString(key) ?? source);
                return new LocalizedHtmlString(resourceKey, result, false, arguments);
            }

            return new LocalizedHtmlString(String.Empty, String.Empty);
        }
    }
}