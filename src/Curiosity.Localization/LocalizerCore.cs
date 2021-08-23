using System;
using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using System.Resources;
using System.Threading;

namespace Curiosity.Localization
{
    /// <summary>
    /// Base class with logic to access resources.
    /// </summary>
    public class LocalizerCore
    {
        protected string DefaultLanguage { get; }
        protected ConcurrentDictionary<string, ConcurrentDictionary<string, string>> CachedStrings { get; }
        protected ResourceManager ResourceManager { get; }

        /// <summary>
        ///
        /// </summary>
        public LocalizerCore(Assembly assembly, LocalizationOptions options)
        {
            var baseName = $"{assembly.GetName().Name}.{options.ResourcesFolder}.{options.ResourcesFileName}";

            DefaultLanguage = options.DefaultLanguage.ToLower();
            CachedStrings = new ConcurrentDictionary<string, ConcurrentDictionary<string, string>>();
            ResourceManager = new ResourceManager(baseName, assembly);

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
                if (!forceReadResources && DefaultLanguage.Equals(currentLanguage, StringComparison.OrdinalIgnoreCase))
                {
                    result = source;
                }
                else
                {
                    result = CachedStrings
                        .GetOrAdd(
                            currentLanguage,
                            (lang) => new ConcurrentDictionary<string, string>())
                        .GetOrAdd(
                            ResourceKeyGenerator.Generate(prefix, source),
                            (key) => ResourceManager.GetString(key) ?? source);
                }

                if (arguments.Length == 0)
                    return result;
               
                return String.Format(result, arguments);
            }

            return String.Empty;
        }
    }
}
