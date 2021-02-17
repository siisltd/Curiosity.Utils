using System;

namespace Curiosity.Tools
{
    /// <summary>
    /// Period of time.
    /// </summary>
    public struct DateTimePeriod
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
        /// <exception cref="ArgumentException">If start date greater than end or time specified without date</exception>
        public DateTimePeriod(
            DateTime startDate,
            DateTime endDate,
            TimeSpan? startTime = null,
            TimeSpan? endTime = null,
            bool dayRange = false)
        {
            Start = startTime.HasValue
                ? startDate.Date.Add(startTime.Value)
                : startDate;
            End = dayRange
                ? endDate.Date.AddDays(1).Subtract(TimeSpan.FromMilliseconds(1))
                : endTime.HasValue
                    ? endDate.Date.Add(endTime.Value)
                    : endDate;

            if (Start > End)
                throw new ArgumentException($"{nameof(Start)} can not be greater than {nameof(End)}");
        }

        public TimeSpan Duration => End - Start;

        public void Deconstruct(out DateTime start, out DateTime end)
        {
            start = Start;
            end = End;
        }
    }
}