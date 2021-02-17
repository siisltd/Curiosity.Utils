using System;

namespace Curiosity.Tools
{
    /// <summary>
    /// Period of time with possible nullable values.
    /// </summary>
    public struct NullableDateTimePeriod
    {
        /// <summary>
        /// Start of period.
        /// </summary>
        public DateTime? Start { get; }
        
        /// <summary>
        /// End of period.
        /// </summary>
        public DateTime? End { get; }
        
        /// <summary>
        /// Period of time.
        /// </summary>
        /// <param name="startDate">Start date of period. If <see cref="startTime"/> is specified, time will not be taken into account</param>
        /// <param name="endDate">End date of period. If <see cref="endTime"/> is specified, time will not be taken into account</param>
        /// <param name="startTime">Time of the day for <see cref="startDate"/></param>
        /// <param name="endTime">Time of the day for <see cref="endDate"/></param>
        /// <param name="dayRange">If period for full day. If <langword name="true"/>, <see cref="endTime"/> will not be taken into account and time will be 23:59:59</param>
        /// <exception cref="ArgumentException">If start date greater than end or time specified without date</exception>
        public NullableDateTimePeriod(
            DateTime? startDate, 
            DateTime? endDate, 
            TimeSpan? startTime = null, 
            TimeSpan? endTime = null,
            bool dayRange = false)
        {
            
            if (startDate.HasValue)
            {
                Start = startTime.HasValue
                    ? startDate.Value.Date.Add(startTime.Value)
                    : startDate.Value;
            }
            else
            {
                if (startTime.HasValue) throw new ArgumentException($"{nameof(startTime)} can not be specified without {nameof(startDate)}");
                Start = default;
            }
            
            if (endDate.HasValue)
            {
                End = endTime.HasValue
                    ? endDate.Value.Date.Add(endTime.Value)
                    : endDate.Value;
                
                if (dayRange)
                {
                    End = End.Value.Date.AddDays(1).Subtract(TimeSpan.FromMilliseconds(1));
                }
            }
            else
            {
                if (endTime.HasValue) throw new ArgumentException($"{nameof(endTime)} can not be specified without {nameof(endDate)}");
                End = default;
            }
            
            
            if (Start.HasValue && End.HasValue && Start > End)
                throw new ArgumentException($"{nameof(Start)} can not be greater than {nameof(End)}");
        }
        
        
        public bool Equals(NullableDateTimePeriod other)
        {
            return Start.Equals(other.Start) && End.Equals(other.End);
        }

        public override bool Equals(object obj)
        {
            return obj is NullableDateTimePeriod other && Equals(other);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return (Start.GetHashCode() * 397) ^ End.GetHashCode();
            }
        }
    }
}