using System;

namespace Curiosity.Tools.Coordinates
{
    /// <summary>
    /// Map url builder.
    /// </summary>
    public static class MapLinkBuilder
    {
        /// <summary>
        /// Builds url to map.
        /// </summary>
        /// <param name="mapType">Map type</param>
        /// <param name="latitude">Latitude</param>
        /// <param name="longitude">Longitude</param>
        /// <returns>Link to map</returns>
        public static string Build(MapType mapType, double latitude, double longitude)
        {
            switch (mapType)
            {
                case MapType.Yandex:
                    // details: (https://yandex.ru/dev/yandex-apps-launch/maps/doc/concepts/yandexmaps-web.html/):
                    // - pt: coordinates of the point separated by commas (the marker will be placed there)
                    // - z: scale (1 - the whole world, 19-the most detailed), take 15-this is a scale of 200 meters
                    // - l: modes (map-scheme, sat-satellite, skl-hybrid)
                    return $"https://yandex.ru/maps/?pt={longitude.ToCoordinateString()},{latitude.ToCoordinateString()}&z=15&l=map";
                default:
                    throw new ArgumentOutOfRangeException(nameof(mapType), mapType, null);
            }
        }
    }
}