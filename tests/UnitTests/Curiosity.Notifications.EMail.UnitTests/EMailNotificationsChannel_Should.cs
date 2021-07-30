using Curiosity.EMail;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Curiosity.Notifications.EMail.UnitTests
{
    /// <summary>
    /// Positive unit tests for <see cref="EmailNotificationChannel"/>.
    /// </summary>
    public class EMailNotificationsChannel_Should
    {
        [Fact]
        public void Ctor_CreateChannel_WithoutRegisteredPostProcessors()
        {
            // Arrange

            // add to Ioc only channel without post processors
            var services = new ServiceCollection();
            services.AddCuriosityEMailChannel();

            // add mocks
            services.AddSingleton(Mock.Of<ILogger<EmailNotificationChannel>>());
            services.AddSingleton(Mock.Of<IEMailSender>());

            var sp = services.BuildServiceProvider();

            // Act
            var channel = sp.GetRequiredService<EmailNotificationChannel>();

            // Assert
            channel.Should().NotBeNull();
        }
    }
}
