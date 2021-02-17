using System;
using System.IO;
using System.Text;

namespace Curiosity.Tools
{
    public static class StringExtensions
    {
        public static string RemoveSpaces(this string source)
        {
            return source.Replace(" ", "");
        }

        public static string? TrimLimit(this string? source, int limit)
        {
            if (limit <= 0) throw new ArgumentOutOfRangeException(nameof(limit));
            if (source == null) return source;

            return source.Length > limit
                ? $"{source.Substring(0, limit - 1)}..."
                : source;
        }
        public static bool IsInt32(this string text)
        {
            return !String.IsNullOrEmpty(text) && Int32.TryParse(text, out _);
        }

        public static bool IsInt64(this string text)
        {
            return !String.IsNullOrEmpty(text) && Int64.TryParse(text, out _);
        }

        public static string? SafeTrim(this string? text)
        {
            return text?.Trim();
        }

        public static string ToSingleLine(this string text)
        {
            var result = new StringBuilder();
            using (var sr = new StringReader(text))
            {
                string? s;
                while ((s = sr.ReadLine()) != null)
                {
                    if ((s = s.NormalizeWhitespaces()).Length > 0)
                    {
                        if (result.Length > 0)
                            result.Append(' ');
                        result.Append(s);
                    }
                }
            }

            return result.ToString();
        }

        public static string NormalizeWhitespaces(this string? text)
        {
            var result = new StringBuilder();
            if (text != null)
            {
                var lastWasWhitespace = false;
                for (var i = 0; i < text.Length; i++)
                {
                    var ch = text[i];
                    if (Char.IsWhiteSpace(ch))
                    {
                        lastWasWhitespace = true;
                    }
                    else
                    {
                        if (lastWasWhitespace)
                        {
                            result.Append(' ');
                        }
                        result.Append(ch);
                        lastWasWhitespace = false;
                    }
                }
            }

            return result.ToString();
        }

        public static string TabsToSpaces(this string text)
        {
            var s = text;
            while (true)
            {
                var idx = s.IndexOf('\t');
                if (idx == -1)
                    break;

                s = s.Substring(0, idx) + ' ' + ((idx + 1 < s.Length) ? s.Substring(idx + 1) : String.Empty);
            }

            return s;
        }

        public static string ToQuoted(this string text, char quoteCh)
        {
            var sc = quoteCh.ToString();
            return sc + text.Replace(sc, sc + sc) + sc;
        }

        public static string ToEllipsis(this String text, int maxLength)
        {
            if (String.IsNullOrEmpty(text))
                return String.Empty;

            return text.Length <= maxLength ? text : (maxLength > 3 ? text.Substring(0, maxLength - 3) : String.Empty) + "...";
        }

        public static string OnlyDigits(this string text)
        {
            var sb = new StringBuilder();
            foreach (var ch in text)
            {
                if (Char.IsDigit(ch))
                    sb.Append(ch);
            }
            return sb.ToString();
        }

        public static bool ContainsSpaces(this string? text)
        {
            if (text != null)
            {
                for (var i = 0; i < text.Length; i++)
                {
                    if (Char.IsWhiteSpace(text, i))
                        return true;
                }
            }

            return false;
        }

        public static bool IsValidWebColor(this string text)
        {
            if (!String.IsNullOrEmpty(text))
            {
                if (text[0] == '#' && text.Length > 1)
                {
                    var color = text.Substring(1).ToLower();
                    if (color.Length == 3 || color.Length == 6)
                    {
                        foreach (var ch in color)
                        {
                            if (!(Char.IsDigit(ch) || (ch >= 'a' && ch <= 'f')))
                            {
                                return false;
                            }
                        }

                        return true;
                    }
                }
            }

            return false;
        }
        
        /// <summary>
        /// Replaces macros in a string
        /// </summary>
        /// <param name="text">Original string</param>
        /// <param name="macro">The macro that you want to replace</param>
        /// <param name="replaceToText">The text that replaces the macro</param>
        /// <returns>
        /// The string that replaces the macro with the specified text
        /// </returns>
        public static string? ReplaceMacro(this string? text, string macro, string replaceToText)
        {
            if (text == null) return null;
            
            var stringIdx = 0;
            while (stringIdx < text.Length)
            {
                var macrosIdx = text.IndexOf(macro, stringIdx, StringComparison.OrdinalIgnoreCase);
                if (macrosIdx == -1)
                    break;

                var macrosIdx1 = macrosIdx + macro.Length;
                text = text.Substring( 0, macrosIdx) 
                       + replaceToText 
                       + ((macrosIdx1 < text.Length)
                           ? text.Substring(macrosIdx1) 
                           : String.Empty);

                stringIdx += replaceToText.Length;
            }

            return text;
        }

        /// <summary>
        /// Limits a string to a specified number of characters
        /// </summary>
        /// <param name="s">Instance of string</param>
        /// <param name="maxLength">Max characters count</param>
        /// <returns></returns>
        public static string? MaxLength(this string? s, int maxLength)
        {
            return s != null && s.Length > maxLength ? s.Substring(0, maxLength) : s;
        }
        
        /// <summary>
        /// Converts string to <see langword="null"/> if empty
        /// </summary>
        /// <param name="str">Source string</param>
        /// <returns>Converted string</returns>
        public static string? ToNullIfEmpty(this string str)
        {
            return !String.IsNullOrEmpty(str) ? str : null;
        }
        
        /// <summary>
        /// Converts string to <see langword="null"/> if empty or <see langword="null"/>
        /// </summary>
        /// <param name="str">Source string</param>
        /// <returns>Converted string</returns>
        public static string? ToNullIfWhiteSpace(this string? str)
        {
            return !String.IsNullOrWhiteSpace(str) ? str : null;
        }

        /// <summary>
        /// Escapes string for LIKE statement in sql queries
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static string? EscapeForSqlLikeSafe(this string? s)
        {
            if (s == null) return null;
            
            var result = new StringBuilder();
            for (var i = 0; i < s.Length; i++)
            {
                var ch = s[i];
                if (ch == '%' || ch == '_' || ch == '\\')
                    result.Append('\\');
                result.Append(ch);
            }
            return result.ToString();
        }
        
        /// <summary>
        /// Replaces unintelligible characters with those that are understandable for the SQL LIKE function
        /// </summary>
        public static string? CleanForSqlLike(this string? s)
        {
            if (s == null) return null;
            
            var result = new StringBuilder();
            for (var i = 0; i < s.Length; i++)
            {
                var ch = s[i];
                switch (ch)
                {
                    case '*':
                        result.Append('%');
                        break;
                        
                    case '?':
                        result.Append('_');
                        break;
                        
                    default:
                        result.Append(ch);
                        break;
                }
            }
            return result.ToString();

        }

        /// <summary>
        /// Adds a string with the result length limit: if the total length of the strings is greater than the limit, the second row overlaps the first one
        /// </summary>
        public static string AppendStringWithOverlap(
            this string sourceString,
            string? appendedString,
            int? resultMaxStringLength)
        {
            if (String.IsNullOrWhiteSpace(sourceString)) throw new ArgumentNullException(nameof(sourceString));

            var sourceStringLength = sourceString.Length;
            var appendedStringLength = appendedString?.Length ?? 0;

            // if there are no restrictions or we fit into them, we will return as-is
            if (!resultMaxStringLength.HasValue || sourceStringLength + appendedStringLength <= resultMaxStringLength.Value)
                return sourceString + appendedString;

            if (appendedStringLength >= resultMaxStringLength.Value) throw new InvalidOperationException("The added string cannot rewrite the original one");

            // if we don't add anything, we will return the original version
            if (appendedString == null) return sourceString;

            // how much can a source have
            var sourceStringMaxLength = resultMaxStringLength.Value - appendedString.Length;

            return (sourceString.Length > sourceStringMaxLength
                       ? sourceString.Substring(0, sourceStringMaxLength)
                       : sourceString)
                   + appendedString;
        }

        /// <summary>
        /// Adds parentheses to the beginning and end
        /// </summary>
        public static string WrapInBrackets(this string? source)
        {
            if (String.IsNullOrWhiteSpace(source)) return String.Empty;

            return $"({source})";
        }
    }
}