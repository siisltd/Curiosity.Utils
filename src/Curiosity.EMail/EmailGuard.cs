using System;
using System.Runtime.CompilerServices;

namespace Curiosity.EMail
{
    /// <summary>
    /// Guard for checking email params.
    /// </summary>
    public static class EmailGuard
    {
        /// <summary>
        /// Checks that toAddress is correct.
        /// </summary>
        /// <param name="value">ToAddress value.</param>
        /// <param name="fieldName">ToAddress field name.</param>
        /// <exception cref="ArgumentNullException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertToAddress(string value, string fieldName = "toAddress")
        {
            if (String.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(fieldName, "ToAddress can't be empty");
        }
        
        /// <summary>
        /// Checks that subject is correct.
        /// </summary>
        /// <param name="value">Subject value.</param>
        /// <param name="fieldName">Subject field name.</param>
        /// <exception cref="ArgumentNullException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertSubject(string value, string fieldName = "subject")
        {
            if (String.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(fieldName, "Subject can't be empty");
        }
        
        /// <summary>
        /// Checks that body is correct.
        /// </summary>
        /// <param name="value">Body value.</param>
        /// <param name="fieldName">Body field name.</param>
        /// <exception cref="ArgumentNullException"></exception>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void AssertBody(string value, string fieldName = "body")
        {
            if (String.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(fieldName, "Body can't be empty");
        }
    }
}
