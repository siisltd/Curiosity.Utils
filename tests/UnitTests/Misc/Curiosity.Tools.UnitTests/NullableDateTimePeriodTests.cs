using System;
using Xunit;

namespace Curiosity.Tools.UnitTests
{
    public class NullableDateTimePeriodTests
    {
        [Fact]
        public void OnlyDates_Ok()
        {
            var startDate = new DateTime(2019, 06, 04);
            var endDate = new DateTime(2019, 06, 05);

            var period = new NullableDateTimePeriod(startDate, endDate);

            Assert.Equal(startDate, period.Start);
            Assert.Equal(endDate, period.End);
        }

        [Fact]
        public void DatesWithSeparateTime_Ok()
        {
            var startDate = new DateTime(2019, 06, 04);
            var startTime = new TimeSpan(11, 00, 00);
            var endDate = new DateTime(2019, 06, 05);
            var endTime = new TimeSpan(12, 00, 00);

            var period = new NullableDateTimePeriod(startDate, endDate, startTime, endTime);

            Assert.Equal(startDate.Add(startTime), period.Start);
            Assert.Equal(endDate.Add(endTime), period.End);
        }

        [Fact]
        public void DatesWithSeparateTime_TimeOfDateErased()
        {
            var startDate = new DateTime(2019, 06, 04, 10, 0, 0);
            var startTime = new TimeSpan(11, 00, 00);
            var endDate = new DateTime(2019, 06, 05, 15, 0, 0);
            var endTime = new TimeSpan(12, 00, 00);

            var period = new NullableDateTimePeriod(startDate, endDate, startTime, endTime);

            Assert.Equal(startDate.Date.Add(startTime), period.Start);
            Assert.Equal(endDate.Date.Add(endTime), period.End);
        }

        [Fact]
        public void OnlyDatesWithDayRange_Ok()
        {
            var startDate = new DateTime(2019, 06, 04);
            var endDate = new DateTime(2019, 06, 05);

            var period = new NullableDateTimePeriod(startDate, endDate, dayRange: true);

            Assert.Equal(startDate, period.Start);
            Assert.Equal(endDate.Date.AddDays(1).Subtract(TimeSpan.FromMilliseconds(1)), period.End);
        }

        [Fact]
        public void StartGreaterEnd_Throws()
        {
            var startDate = new DateTime(2019, 06, 04);
            var endDate = new DateTime(2019, 06, 05);

            try
            {
                var period = new NullableDateTimePeriod(endDate, startDate);
                Assert.True(false);
            }
            catch
            {
                // ignored
            }
        }

        [Fact]
        public void DatesWithSeparateTimeWithDayRange_Ok()
        {
            var startDate = new DateTime(2019, 06, 04);
            var startTime = new TimeSpan(11, 00, 00);
            var endDate = new DateTime(2019, 06, 05);
            var endTime = new TimeSpan(12, 00, 00);

            var period = new NullableDateTimePeriod(startDate, endDate, startTime, endTime, true);

            Assert.Equal(startDate.Add(startTime), period.Start);
            Assert.Equal(endDate.Date.AddDays(1).Subtract(TimeSpan.FromMilliseconds(1)), period.End);
        }

        [Fact]
        public void NoDateTimes_AllNull()
        {
            var period = new NullableDateTimePeriod(null, null, null, null, true);

            Assert.Equal(default, period.Start);
            Assert.Equal(default, period.End);
        }

        [Fact]
        public void NoStartDateTimes_AllNull()
        {
            var endDate = new DateTime(2019, 06, 05);
            var period = new NullableDateTimePeriod(null, endDate);

            Assert.Equal(default, period.Start);
            Assert.Equal(endDate, period.End);
        }

        [Fact]
        public void NoEndDateTimes_AllNull()
        {
            var startDate = new DateTime(2019, 06, 05);
            var period = new NullableDateTimePeriod(startDate, null);

            Assert.Equal(startDate, period.Start);
            Assert.Equal(default, period.End);
        }

        [Fact]
        public void NoStartDateButHaveTimes_Throw()
        {
            var startTime = new TimeSpan(11, 00, 00);
            var endDate = new DateTime(2019, 06, 05);
            var endTime = new TimeSpan(12, 00, 00);

            try
            {
                var period = new NullableDateTimePeriod(null, endDate, startTime, endTime, true);
            }
            catch
            {
                // ignored
            }
        }

        [Fact]
        public void NoEndDateButHaveTimes_Throw()
        {
            var startDate = new DateTime(2019, 06, 04);
            var startTime = new TimeSpan(11, 00, 00);
            var endTime = new TimeSpan(12, 00, 00);

            try
            {
                var period = new NullableDateTimePeriod(startDate, null, startTime, endTime, true);
            }
            catch
            {
                // ignored
            }
        }
    }
}