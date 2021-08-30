using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
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

        [Fact]
        public async Task Notificator_SendAsync()
        {
            // arrange
            // create sender mock
            var isSenderCalled = false;

            var senderMock = new Mock<IEMailSender>();
            senderMock
                .Setup(x => x.SendAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .Callback(() =>
                {
                    isSenderCalled = true;
                });

            // add to Ioc only channel without post processors
            var services = new ServiceCollection();
            services.AddCuriosityEMailChannel();

            // add mocks
            services.AddSingleton(Mock.Of<ILogger<EmailNotificationChannel>>());
            services.AddSingleton(Mock.Of<ILogger>());
            services.AddSingleton(senderMock.Object);
            services.AddSingleton<INotificationBuilder, TestBuilder>();

            var sp = services.BuildServiceProvider();
            await sp.GetService<EmailNotificationChannel>().StartAsync(default);

            // act
            await sp.GetService<INotificator>().NotifyAsync(new TestMetadata());

            // assert
            isSenderCalled.Should().BeTrue();
        }

        public class TestMetadata : INotificationMetadata
        {
        }

        public class TestBuilder : EmailNotificationBuilderBase<TestMetadata>
        {
            protected override Task<IReadOnlyList<INotification>> BuildNotificationsAsync(TestMetadata metadata, CancellationToken cancellationToken = default)
            {
                return Task.FromResult((IReadOnlyList<INotification>) new INotification[] {new EmailNotification("email", "subject", "body")});
            }
        }
    }
}
