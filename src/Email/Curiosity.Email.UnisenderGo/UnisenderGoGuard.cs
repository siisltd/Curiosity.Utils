using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Curiosity.Email.UnisenderGo
{
    /// <summary>
    /// Guard for checking UnisenderGo email sender params.
    /// </summary>
    public static class UnisenderGoGuard
    {
        /// <summary>
        /// Checks that API key is correct.
        /// </summary>
        /// <param name="value">API key value.</param>
        /// <param name="fieldName">API key field name.</param>
        /// <exception cref="ArgumentNullException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertApiKey(string value, string fieldName = "apiKey")
        {
            if (String.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(fieldName, "API key can't be empty");
        }

        /// <summary>
        /// Checks that email from is correct.
        /// </summary>
        /// <param name="value">Email from value.</param>
        /// <param name="fieldName">Email from field name.</param>
        /// <exception cref="ArgumentNullException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertEmailFrom(string value, string fieldName = "emailFRom")
        {
            if (String.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(fieldName, "Email from can't be empty");
        }

        /// <summary>
        /// Checks that email from name is correct.
        /// </summary>
        /// <param name="value">Email from name value.</param>
        /// <param name="fieldName">Email from name field name.</param>
        /// <exception cref="ArgumentNullException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertFromName(string value, string fieldName = "fromName")
        {
            if (String.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(fieldName, "Email from name can't be empty");
        }

        /// <summary>
        /// Checks that region is correct.
        /// </summary>
        /// <param name="value">Region value.</param>
        /// <param name="fieldName">Region field name.</param>
        /// <exception cref="ArgumentNullException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertRegion(UnisenderGoRegion value, string fieldName = "region")
        {
            if (!Enum.IsDefined(typeof(UnisenderGoRegion), value)) throw new InvalidEnumArgumentException(fieldName, (int)value, typeof(UnisenderGoRegion));
        }
    }
}
