using System;

namespace Curiosity.Tools
{
    /// <summary>
    /// Period of time.
    /// </summary>
    public struct DateTimePeriod : IEquatable<DateTimePeriod>
    {
        /// <summary>
        /// Start of period.
        /// </summary>
        public DateTime Start { get; }

        /// <summary>
        /// End of period.
        /// </summary>
        public DateTime End { get; }

        /// <summary>
        /// Period of time.
        /// </summary>
        /// <param name="startDate">Start date of period. If <see cref="startTime"/> is specified, time will not be taken into account</param>
        /// <param name="endDate">End date of period. If <see cref="endTime"/> is specified, time will not be taken into account</param>
        /// <param name="startTime">Time of the day for <see cref="startDate"/></param>
        /// <param name="endTime">Time of the day for <see cref="endDate"/></param>
        /// <param name="dayRange">If period for full day. If <langword name="true"/>, <see cref="endTime"/> will not be taken into account and time will be 23:59:59</param>
        /// <param name="trimMilliseconds">Trims millisecond for result dates when true</param>
        /// <exception cref="ArgumentException">If start date greater than end or time specified without date</exception>
        public DateTimePeriod(
            DateTime startDate,
            DateTime endDate,
            TimeSpan? startTime = null,
            TimeSpan? endTime = null,
            bool dayRange = false,
            bool trimMilliseconds = false)
        {
            Start = startTime.HasValue
                ? startDate.Date.Add(startTime.Value)
                : startDate;
            End = dayRange
                ? endDate.Date.AddDays(1).Subtract(TimeSpan.FromMilliseconds(1))
                : endTime.HasValue
                    ? endDate.Date.Add(endTime.Value)
                    : endDate;

            if (trimMilliseconds)
            {
                Start = Start.TrimMilliseconds();
                End = End.TrimMilliseconds();
            }

            if (Start > End)
                throw new ArgumentException($"{nameof(Start)} can not be greater than {nameof(End)}");
        }

        /// <summary>
        /// Duration of period.
        /// </summary>
        /// <remarks>
        /// This is method because we don't want to make this field serializable. This field is calculable.
        /// </remarks>
        public TimeSpan GetDuration() => End - Start;

        /// <summary>
        /// Deconstructs period into separate start and end date times.
        /// </summary>
        public void Deconstruct(out DateTime start, out DateTime end)
        {
            start = Start;
            end = End;
        }

        /// <summary>
        /// Checks if period contains specified date time.
        /// </summary>
        public bool Contains(DateTime dateTime)
        {
            return Start <= dateTime && dateTime <= End;
        }

        /// <summary>
        /// Checks if current period contains specified period.
        /// </summary>
        public bool Contains(DateTimePeriod period)
        {
            return Start <= period.Start && period.End <= End;
        }

        /// <summary>
        /// Checks if current period overlaps with specified period.
        /// </summary>
        public bool Overlaps(DateTimePeriod period, bool isStrict = true)
        {
            return isStrict
                ? Start <= period.Start && period.Start <= End || Start <= period.End && period.End <= End
                : Start < period.Start && period.Start < End || Start < period.End && period.End < End;
        }

        #region Equals methods

        /// <inheritdoc />
        public bool Equals(DateTimePeriod other)
        {
            return Start.Equals(other.Start) && End.Equals(other.End);
        }

        /// <inheritdoc />
        public override bool Equals(object? obj)
        {
            return obj is DateTimePeriod other && Equals(other);
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return HashCode.Combine(Start, End);
        }

        public static bool operator ==(DateTimePeriod left, DateTimePeriod right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(DateTimePeriod left, DateTimePeriod right)
        {
            return !left.Equals(right);
        }

        #endregion
    }
}
