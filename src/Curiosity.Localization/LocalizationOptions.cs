namespace Curiosity.Localization
{
    /// <summary>
    /// Options for configuring localization.
    /// </summary>
    public class LocalizationOptions
    {
        /// <summary>
        /// Folder in project with the resources.
        /// </summary>
        public string ResourcesFolder { get; set; } = "Resources";
        
        /// <summary>
        /// Resources file name.
        /// </summary>
        public string ResourcesFileName { get; set; } = "Strings";
        
        /// <summary>
        /// Resource key prefix, if it's not set explicitly.
        /// </summary>
        public string Prefix { get; set; } = "ssng";
        
        /// <summary>
        /// Default language.
        /// </summary>
        public string DefaultLanguage { get; set; } = "ru";
        
        /// <summary>
        /// Supported languages, except default language.
        /// </summary>
        public string[] SupportedLanguages { get; set; } = new[] { "en" };
        
        /// <summary>
        /// Should check if files with resources is existing for supported languages.
        /// </summary>
        public bool CheckResourceFiles { get; set; } = false;
    }
}
