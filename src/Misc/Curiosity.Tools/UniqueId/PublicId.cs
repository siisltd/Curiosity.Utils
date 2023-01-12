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
            return id.ToString("x16", CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Converts an ID from a "public" view to a regular long (our internal)
        /// </summary>
        /// <param name= "exportId" >Public version of the ID (can be either hex or regular)</param>
        /// <param name= " id " >Our regular ID</param>
        /// <returns>Successfully parted?</returns>
        public static bool TryParse(string exportId, out long id)
        {
            id = 0;
            if (String.IsNullOrWhiteSpace(exportId))
                return false;

            // first we will try to parse as a decimal number, and then as hex
            return long.TryParse(exportId, out id) ||
                   long.TryParse(exportId, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out id);
        }
    }
}