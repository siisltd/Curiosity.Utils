using System;
using System.Collections.Generic;
using System.Text;

namespace Curiosity.DAL.NpgSQL
{
    /// <summary>
    /// Extension method for Like/ILike functions at Postgres.
    /// </summary>
    public static class LikeFunctionHelpers
    {
        /// <summary>
        /// Special characters for Like/ILike.
        /// </summary>
        public static readonly char[] SpecialChars = {
            '%',
            '|', 
            '(',
            ')',
            '*',
            '+',
            '?',
            '{',
            '}',
            '[',
            ']',
            '_',
            '~',
            '!'
        };

        private static readonly HashSet<char> SpecialCharsMap = new HashSet<char>(SpecialChars);
        
        /// <summary>
        /// Removes special chars from pattern (<see cref="SpecialChars"/>).
        /// </summary>
        public static string RemoveSpecialChars(string pattern)
        {
            if (String.IsNullOrWhiteSpace(pattern)) return String.Empty;

            var stringBuilder = new StringBuilder(pattern.Length);

            for (var i = 0; i < pattern.Length; ++i)
            {
                if (SpecialCharsMap.Contains(pattern[i])) continue;

                stringBuilder.Append(pattern[i]);
            }
            
            return stringBuilder.ToString();
        }

        /// <summary>
        /// Checks if pattern contains special chars (<see cref="SpecialChars"/>).
        /// </summary>
        public static bool ContainsSpecialChars(string pattern)
        {
            for (var i = 0; i < pattern.Length; ++i)
            {
                if (SpecialCharsMap.Contains(pattern[i])) return true;
            }

            return false;
        }
    }
}