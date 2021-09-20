using System;

namespace Curiosity.Tools
{
    /// <summary>
    /// Class to generate unique url.
    /// </summary>
    public static class UniqueUrlGenerator
    {
        private static readonly char[] BaseChars =
            "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();

        /// <summary>
        /// Generates unique url path.
        /// </summary>
        public static string GenerateUniqueUrlPath()
        {
            // generate unique id
            var uniqueId = UniqueIdGenerator.Generate();

            // convert id from DEC to out custom system (0-9A-Za-z)
            long targetBase = BaseChars.Length;
            char[] buffer = new char[Math.Max(
                (int) Math.Ceiling(Math.Log(uniqueId + 1, targetBase)), 1)];

            var i = buffer.Length;
            do
            {
                buffer[--i] = BaseChars[uniqueId % targetBase];
                uniqueId = uniqueId / targetBase;
            }
            while (uniqueId > 0);

            return new string(buffer, i, buffer.Length - i);
        }
    }
}
