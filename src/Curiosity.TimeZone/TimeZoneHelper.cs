using System;
using System.Collections.Generic;
using NodaTime;

namespace Curiosity.TimeZone
{
    /// <summary>
    /// Helper class for time zones.
    /// </summary>
    public class TimeZoneHelper
    {
        private static readonly Dictionary<string, string> TimeZoneDisplayNames;
        
        static TimeZoneHelper()
        {
            TimeZoneDisplayNames = new Dictionary<string, string>(DateTimeZoneProviders.Tzdb.Ids.Count);
            for (var i = 0; i < DateTimeZoneProviders.Tzdb.Ids.Count; i++)
            {
                var id = DateTimeZoneProviders.Tzdb.Ids[i];
                TimeZoneDisplayNames[id] = GetTimeZoneDisplayName(id);
            }
        }
        
        /// <summary>
        /// Returns a list with human readable names of existing time zones.
        /// </summary>
        public Dictionary<string, string> GetTimeZoneDisplayNames()
        {
            return new Dictionary<string, string>(TimeZoneDisplayNames);
        }
        
        /// <summary>
        /// Returns readable time zone name
        /// </summary>
        /// <param name="timeZoneId">Time zone code</param>
        public static string GetTimeZoneDisplayName(string timeZoneId)
        {
            var tz = DateTimeZoneProviders.Tzdb.GetZoneOrNull(timeZoneId);
            if (tz == null) return $"{timeZoneId} (UNKNOWN)";
            
            var utcOffset = tz.GetUtcOffset(Instant.FromDateTimeUtc(DateTime.UtcNow)).Milliseconds / 1000 / 60;
            var utcOffsetAbs = Math.Abs(utcOffset);
            var utcStr = utcOffset != 0 ? $"{(utcOffset < 0 ? "-" : "+")}{utcOffsetAbs / 60:d2}:{utcOffsetAbs % 60:d2}"
                : String.Empty;
            
            return $"{tz.Id} (UTC{utcStr})";
        }
        
        /// <summary>
        /// Converts time from UTC to time in specified time zone.
        /// </summary>
        /// <param name="utc">UTC time.</param>
        /// <param name="timeZoneId">Specified time zone.</param>
        /// <returns>Time at specified time zone</returns>
        public DateTime ToClientTime(string timeZoneId, DateTime utc)
        {
            return ToClientTime(GetTimeZone(timeZoneId), utc);
        }
        
        /// <summary>
        /// Converts time from UTC to time in specified time zone.
        /// </summary>
        /// <param name="utc">UTC time.</param>
        /// <param name="timeZone">Specified time zone.</param>
        /// <returns>Time at specified time zone</returns>
        public DateTime ToClientTime(DateTimeZone timeZone, DateTime utc)
        {
            return Instant
                .FromDateTimeUtc(DateTime.SpecifyKind(utc, DateTimeKind.Utc))
                .InZone(timeZone)
                .ToDateTimeUnspecified();
        }
        
        /// <summary>
        /// Converts from client time to UTC.
        /// </summary>
        /// <param name="zone">Client time zone.</param>
        /// <param name="clientTime">Client time.</param>
        /// <returns></returns>
        public DateTime FromClientTime(DateTimeZone zone, DateTime clientTime)
        {
            return zone.AtLeniently(LocalDateTime.FromDateTime(clientTime)).ToDateTimeUtc();
        }
        
        /// <summary>
        /// Returns time zone by id.
        /// </summary>
        /// <param name="timeZoneId">Time zone id.</param>
        /// <returns></returns>
        public DateTimeZone GetTimeZone(string timeZoneId)
        {
            return DateTimeZoneProviders.Tzdb.GetZoneOrNull(timeZoneId) ?? DateTimeZone.Utc;
        }

        /// <summary>
        /// Returns true when the NodaTime can get a time zone from time zone id
        /// </summary>
        /// <param name="timeZoneId">Time zone id.</param>
        public static bool IsTimeZoneIdValid(string? timeZoneId)
        {
            if (timeZoneId == null) return false;
            
            return DateTimeZoneProviders.Tzdb.GetZoneOrNull(timeZoneId) != null;
        }
        
        /// <summary>
        /// Returns time zone by ISO code of Russia regions.
        /// Returns null, when time zine not found.
        /// </summary>
        public static string? FromRuRegion(string? isoCode)
        {
            return isoCode switch
            {
                "RU-AD" => "Europe/Moscow",
                "RU-AL" => "Asia/Omsk",
                "RU-BA" => "Asia/Yekaterinburg",
                "RU-BU" => "Asia/Irkutsk",
                "RU-CE" => "Europe/Moscow",
                "RU-CU" => "Europe/Moscow",
                "RU-DA" => "Europe/Moscow",
                "RU-IN" => "Europe/Moscow",
                "RU-KB" => "Europe/Moscow",
                "RU-KL" => "Europe/Moscow",
                "RU-KC" => "Europe/Moscow",
                "RU-KR" => "Europe/Moscow",
                "RU-KK" => "Asia/Krasnoyarsk",
                "RU-KO" => "Europe/Moscow",
                "RU-ME" => "Europe/Moscow",
                "RU-MO" => "Europe/Moscow",
                "RU-SA" => "Asia/Yakutsk",
                "RU-SE" => "Europe/Moscow",
                "RU-TA" => "Europe/Moscow",
                "RU-TY" => "Asia/Krasnoyarsk",
                "RU-UD" => "Europe/Samara",
                "RU-ALT" => "Asia/Novosibirsk",
                "RU-KAM" => "Asia/Kamchatka",
                "RU-KHA" => "Asia/Vladivostok",
                "RU-KDA" => "Europe/Moscow",
                "RU-KYA" => "Asia/Krasnoyarsk",
                "RU-PER" => "Asia/Yekaterinburg",
                "RU-PRI" => "Asia/Vladivostok",
                "RU-STA" => "Europe/Moscow",
                "RU-ZAB" => "Asia/Chita",
                "RU-AMU" => "Asia/Chita",
                "RU-ARK" => "Europe/Moscow",
                "RU-AST" => "Europe/Astrakhan",
                "RU-BEL" => "Europe/Moscow",
                "RU-BRY" => "Europe/Moscow",
                "RU-CHE" => "Asia/Yekaterinburg",
                "RU-IRK" => "Asia/Irkutsk",
                "RU-IVA" => "Europe/Moscow",
                "RU-KGD" => "Europe/Kaliningrad",
                "RU-KLU" => "Europe/Moscow",
                "RU-KEM" => "Asia/Novokuznetsk",
                "RU-KIR" => "Europe/Moscow",
                "RU-KOS" => "Europe/Moscow",
                "RU-KGN" => "Asia/Yekaterinburg",
                "RU-KRS" => "Europe/Moscow",
                "RU-LEN" => "Europe/Moscow",
                "RU-LIP" => "Europe/Moscow",
                "RU-MAG" => "Asia/Magadan",
                "RU-MOS" => "Europe/Moscow",
                "RU-MUR" => "Europe/Moscow",
                "RU-NIZ" => "Europe/Moscow",
                "RU-NGR" => "Europe/Moscow",
                "RU-NVS" => "Asia/Novosibirsk",
                "RU-OMS" => "Asia/Omsk",
                "RU-ORE" => "Europe/Moscow",
                "RU-ORL" => "Asia/Yekaterinburg",
                "RU-PNZ" => "Europe/Moscow",
                "RU-PSK" => "Europe/Moscow",
                "RU-ROS" => "Europe/Moscow",
                "RU-RYA" => "Europe/Moscow",
                "RU-SAK" => "Asia/Sakhalin",
                "RU-SAM" => "Europe/Samara",
                "RU-SAR" => "Europe/Saratov",
                "RU-SMO" => "Europe/Moscow",
                "RU-SVE" => "Asia/Yekaterinburg",
                "RU-TAM" => "Europe/Moscow",
                "RU-TOM" => "Asia/Tomsk",
                "RU-TUL" => "Europe/Moscow",
                "RU-TVE" => "Europe/Moscow",
                "RU-TYU" => "Asia/Yekaterinburg",
                "RU-ULY" => "Europe/Ulyanovsk",
                "RU-VLA" => "Europe/Moscow",
                "RU-VGG" => "Europe/Moscow",
                "RU-VLG" => "Europe/Moscow",
                "RU-VOR" => "Europe/Moscow",
                "RU-YAR" => "Europe/Moscow",
                "RU-MOW" => "Europe/Moscow",
                "RU-SPE" => "Europe/Moscow",
                "RU-YEV" => "Asia/Vladivostok",
                "RU-CHU" => "Asia/Anadyr",
                "RU-KHM" => "Asia/Yekaterinburg",
                "RU-NEN" => "Europe/Moscow",
                "RU-YAN" => "Asia/Yekaterinburg",
                "UA-43" => "Europe/Moscow",
                "UA-40" => "Europe/Moscow",
                _ => null,
            };
    }
}