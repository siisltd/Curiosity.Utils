using System;
using System.Globalization;

namespace Curiosity.Tools
{
    /// <summary>
    /// Extension methods for working with our unique Id
    /// </summary>
    public static class PublicId
    {
        /// <summary>
        /// Converts the ID to a" public " view (for export to upload and display to the client)
        /// </summary>
        /// <param name="id" >Source ID</param>
        /// <returns>ID in HEX</returns>
        public static string ToPublicId(this long id)
        {
            // number 17 in the format indicates the minimum number of characters in the string
            // missing characters are replaced with 0
            // by the first 0, we can detect that the number is hexadecimal, even if all numbers are there
            // until 2024 year, our UniqueIdGenerator had generated 16 characters string
            return id.ToString("x17", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts an ID from a "public" view to a regular long.
        /// </summary>
        /// <param name= "exportId" >String number can be hex or dec</param>
        /// <param name= "id" >Our regular ID</param>
        /// <returns>Successfully parted?</returns>
        public static bool TryParse(string exportId, out long id)
        {
            id = 0;
            if (String.IsNullOrWhiteSpace(exportId))
                return false;

            // try parse as hex or dec
            return IsHexFormat(exportId)
                ? long.TryParse(exportId, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out id)
                : long.TryParse(exportId, out id);
        }

        /// <summary>
        /// Detects that line format is hex or dec
        /// </summary>
        private static bool IsHexFormat(string line)
        {
            // hex when contains non numeric symbols
            for (var i = 0; i < line.Length; i++)
            {
                if (!char.IsDigit(line[i]))
                    return true;
            }
        
            // until 2024 year, our UniqueIdGenerator had generated 16 chars string then 17 chars
            // always with first 0
            if ((line.Length == 16 || line.Length == 17) && 
                line[0] == '0')
                return true;

            // exactly dec
            return false;
        }
    }
}