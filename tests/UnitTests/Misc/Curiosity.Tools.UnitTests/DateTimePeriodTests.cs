using System;
using Xunit;

namespace Curiosity.Tools.UnitTests
{
     public class DateTimePeriodTests
    {
        [Fact]
        public void OnlyDates_Ok()
        {
            var startDate = new DateTime(2019, 06, 04);
            var endDate = new DateTime(2019, 06, 05);

            var period = new DateTimePeriod(startDate, endDate);

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

            var period = new DateTimePeriod(startDate, endDate, startTime, endTime);

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

            var period = new DateTimePeriod(startDate, endDate, startTime, endTime);

            Assert.Equal(startDate.Date.Add(startTime), period.Start);
            Assert.Equal(endDate.Date.Add(endTime), period.End);
        }

        [Fact]
        public void OnlyDatesWithDayRange_Ok()
        {
            var startDate = new DateTime(2019, 06, 04);
            var endDate = new DateTime(2019, 06, 05);

            var period = new DateTimePeriod(startDate, endDate, dayRange: true);

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
                var period = new DateTimePeriod(endDate, startDate);
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

            var period = new DateTimePeriod(startDate, endDate, startTime, endTime, true);

            Assert.Equal(startDate.Add(startTime), period.Start);
            Assert.Equal(endDate.Date.AddDays(1).Subtract(TimeSpan.FromMilliseconds(1)), period.End);
        }
    }
}
