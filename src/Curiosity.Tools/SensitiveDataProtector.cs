using System;
using System.Text.RegularExpressions;

namespace Curiosity.Tools
{
    /// <summary>
    /// Hides sensitive customer data
    /// </summary>
    /// <remarks>
    /// Use it to hide passwords in loggable requests.
    /// </remarks>
    public class SensitiveDataProtector
    {
        /// <summary>
        /// Name collection of the fields the data in which we want to hide
        /// </summary>
        private readonly string[] _sensitiveFieldNames;

        public SensitiveDataProtector(params string[] sensitiveFieldNames)
        {
            _sensitiveFieldNames = sensitiveFieldNames ?? throw new ArgumentNullException(nameof(sensitiveFieldNames));
        }
        
        /// <summary>
        /// Hide data in JSON
        /// </summary>
        public string? HideInJson(string? json)
        {
            if (json == null) return null;

            foreach (var name in _sensitiveFieldNames)
            {
                json = Regex.Replace(
                    json, 
                    $"(\"{name}\":\\s*\".*\")+", 
                    $"\"{name}\": ***", 
                    RegexOptions.IgnoreCase);
            }

            return json;
        }
    }
}