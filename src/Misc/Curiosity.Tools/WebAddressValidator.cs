using System;
using System.Text.RegularExpressions;

namespace Curiosity.Tools
{
    /// <summary>
    /// Web address validator
    /// </summary>
    public static class WebAddressValidator
    {
        private const string IPWithPortRegexPattern =
           @"^(([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])\.){3}([0-9]|[1-9][0-9]|1[0-9]{2}|2[0-4][0-9]|25[0-5])(:[0-9]+)?$";

        private static readonly Regex IPWithPortRegex = new Regex(IPWithPortRegexPattern);
        
        /// <summary>
        /// Checks whether the specified web address is valid (http/https/ip)
        /// </summary>
        public static bool IsValid(string webAddress)
        {
            if (String.IsNullOrWhiteSpace(webAddress)) return false;

            if (Uri.IsWellFormedUriString(webAddress, UriKind.RelativeOrAbsolute)) return true;

            return IPWithPortRegex.IsMatch(webAddress);
        }
    }
}