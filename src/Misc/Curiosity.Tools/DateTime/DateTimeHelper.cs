using System;
using System.Threading;

namespace Curiosity.Tools
{
    public static class DateTimeHelper
    {
        public static string ToMinSec(this int? secs, string? defaultValue = null)
        {
            return secs.HasValue
                ? $"{secs.Value / 60:d2}:{secs.Value % 60:d2}"
                : defaultValue ?? String.Empty;
        }

        public static string ToMinSec(this int secs)
        {
            return $"{secs / 60:d2}:{secs % 60:d2}";
        }

        public static string ToHourMin(this int secs)
        {
            return ToMinSec(secs / 60);
        }

        public static string ToHourMin(this int? secs, string? defaultValue = null)
        {
            return ToMinSec(secs / 60, defaultValue);
        }

        public static string ToMinSec(this long? secs, string? defaultValue = null)
        {
            return secs.HasValue
                ? $"{secs.Value / 60:d2}:{secs.Value % 60:d2}"
                : defaultValue ?? String.Empty;
        }

        public static string ToMinSec(this long secs)
        {
            return $"{secs / 60:d2}:{secs % 60:d2}";
        }

        public static string ToHourMin(this long secs)
        {
            return ToMinSec(secs / 60);
        }

        public static string ToHourMin(this long? secs, string? defaultValue = null)
        {
            return ToMinSec(secs / 60, defaultValue);
        }

        public static string ToShortTimeString(this TimeSpan ts)
        {
            return new DateTime(ts.Ticks).ToShortTimeString();
        }

        public static string ToShortTimeString(this TimeSpan? ts, string? defaultValue = null)
        {
            return ts != null
                ? new DateTime(ts.Value.Ticks).ToShortTimeString()
                : defaultValue ?? String.Empty;
        }

        public static string ToShortLocalizedDateString(this DateTime? dateTime, string? defaultValue = null)
        {
            return dateTime == null
                ? defaultValue ?? String.Empty
                : dateTime.Value.ToShortLocalizedDateString();
        }

        public static string ToShortLocalizedDateString(this DateTime dateTime)
        {
            return dateTime.ToString("d", Thread.CurrentThread.CurrentUICulture);
        }

        public static string ToShortInvariantTimeString(this TimeSpan? timeSpan, string? defaultValue = null)
        {
            return timeSpan == null
                ? defaultValue ?? String.Empty
                : timeSpan.Value.ToShortInvariantTimeString();
        }
        
        public static string ToShortInvariantTimeString(this TimeSpan timeSpan)
        {
            return timeSpan.ToString("hh\\:mm");
        }
        
        public static string ToShortLocalizedDateTimeString(this DateTime dateTime)
        {
            return $"{dateTime.ToShortLocalizedDateString()} {dateTime.TimeOfDay.ToShortInvariantTimeString()}";
        }

        public static DateTime TrimMilliseconds(this DateTime date)
        {
            return new DateTime((date.Ticks / TimeSpan.TicksPerSecond) * TimeSpan.TicksPerSecond);
        }
    }
}