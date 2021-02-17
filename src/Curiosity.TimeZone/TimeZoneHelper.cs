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
    }
}