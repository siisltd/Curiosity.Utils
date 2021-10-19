using System;
using System.Text;
using System.Threading;

namespace Curiosity.Tools
{
    /// <summary>
    /// Class to generate unique keys.
    /// </summary>
    public static class UniqueKeyGenerator
    {        /// <summary>
        /// Seed value for random instance
        /// </summary>
        private static int _randomSeed = Environment.TickCount;

        /// <summary>
        /// Random instance 
        /// </summary>
        /// <remarks>
        /// ThreadLocal allows us to make a different version of static variable for each thread.
        /// Since we usually use a thread pool redundant instances would not be created.
        /// ThreadLocal allows us not to do lock, while being faster and thread-safe
        /// </remarks>
        private static readonly ThreadLocal<Random> _random = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref _randomSeed)));

        private static readonly char[] BaseChars =
            "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".ToCharArray();

        /// <summary>
        /// Generates unique key in base62 encoding, using id from <see cref="UniqueIdGenerator"/>.
        /// </summary>
        public static string GenerateUniqueSequentialKey()
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

        /// <summary>
        /// Generates key with specified length in base62 encoding, using random number.
        /// </summary>
        /// <param name="keyLength">Length of generated key.</param>
        /// <remarks>
        /// But its' better to check for duplicates.
        /// 5 chars in base 62 will give you 62^5 unique IDs = 916,132,832 (~1 billion) At 10k IDs per day you will be ok for 91k+ days.
        /// 6 chars in base 62 will give you 62^6 unique IDs = 56,800,235,584 (56+ billion) At 10k IDs per day you will be ok for 5+ million days.
        /// </remarks>
        public static string GenerateRandomKey(int keyLength)
        {
            var sb = new StringBuilder(keyLength);

            for (int i = 0; i < keyLength; i++)
            {
                sb.Append(BaseChars[_random.Value.Next(BaseChars.Length)]);
            }

            return sb.ToString();
        }
    }
}
