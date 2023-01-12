using System.Globalization;

namespace Curiosity.Tools.Coordinates
{
    public static class DoubleExtensions
    {
        /// <summary>
        /// Converts the coordinate into a valid string that can be used for forming links, working with maps, etc.
        /// </summary>
        public static string ToCoordinateString(this double coordinate)
        {
            return coordinate.ToString("G17", CultureInfo.InvariantCulture);
        }
    }
}