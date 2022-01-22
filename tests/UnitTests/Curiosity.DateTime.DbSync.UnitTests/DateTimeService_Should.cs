using System;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Logging;
using Curiosity.DAL;
using Moq;
using Xunit;

namespace Curiosity.DateTime.DbSync.UnitTests
{
    public class DateTimeService_Should
    {
        [Fact]
        public async Task ReturnCurrentTime_WithPositiveCorrection()
        {
            var timeShift = TimeSpan.FromMinutes(5);
            
            var contextMock = new Mock<ICuriosityDataContext>();
            contextMock
                .Setup(c => c.GetImmediateServerTimeUtcAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(System.DateTime.UtcNow + timeShift));
            
            var contextFactoryMock = new Mock<ICuriosityDataContextFactory>();
            contextFactoryMock
                .Setup(x => x.CreateContext(It.IsAny<bool>()))
                .Returns(contextMock.Object);

            
            var service = new DbSyncDateTimeService(contextFactoryMock.Object);
            await service.InitAsync();
            
            var serverTimeUtc = service.GetCurrentTimeUtc();
            var localTime = System.DateTime.UtcNow;
            var serverTimeWithoutShift = serverTimeUtc - timeShift;
            
            // убеждаемся, что погрешность принебрежимо мала 
            Assert.True(Math.Abs((localTime - serverTimeWithoutShift).Ticks) <= TimeSpan.FromMilliseconds(1000).Ticks);
            Assert.Equal(DateTimeKind.Utc, serverTimeUtc.Kind);
        }        
        
        [Fact]
        public async Task ReturnCurrentTime_WithNegativeCorrection()
        {
            var timeShift = TimeSpan.FromMinutes(5);
            
            var contextMock = new Mock<ICuriosityDataContext>();
            contextMock
                .Setup(c => c.GetImmediateServerTimeUtcAsync(It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(System.DateTime.UtcNow - timeShift));
            
            var contextFactoryMock = new Mock<ICuriosityDataContextFactory>();
            contextFactoryMock
                .Setup(x => x.CreateContext(It.IsAny<bool>()))
                .Returns(contextMock.Object);
            
            var service = new DbSyncDateTimeService(contextFactoryMock.Object);
            await service.InitAsync();

            var serverTimeUtc = service.GetCurrentTimeUtc();
            var localTime = System.DateTime.UtcNow;
            var serverTimeWithoutShift = serverTimeUtc + timeShift;
            
            // убеждаемся, что погрешность принебрежимо мала 
            Assert.True(Math.Abs((localTime - serverTimeWithoutShift).Ticks) <= TimeSpan.FromMilliseconds(1000).Ticks);
            Assert.Equal(DateTimeKind.Utc, serverTimeUtc.Kind);
        }
    }
}