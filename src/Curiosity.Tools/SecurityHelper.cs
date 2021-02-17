using System;
using System.Security.Cryptography;
using System.Text;

namespace Curiosity.Tools
{
    public static class SecurityHelper
    {
        /// <summary>
        /// Calculates the checksum of a string using the MD5 algorithm
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        public static string ComputeHashMD5(this string text)
        {
            return MD5.Create().ComputeHash(Encoding.Default.GetBytes(text)).ToHexString();
        }

        private static string ToHexString(this byte[] bytes)
        {
            string hex = String.Empty;
            foreach (byte b in bytes)
            {
                hex += String.Format("{0:x2}", b);
            }
            return hex;
        }

        /// <summary>
        /// Converts Guid to to string
        /// </summary>
        /// <param name="guid"></param>
        /// <returns></returns>
        public static string ToHexString(this Guid guid)
        {
            return guid.ToByteArray().ToHexString();
        }
    }
}