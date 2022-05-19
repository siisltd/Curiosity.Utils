using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Curiosity.Email.UnisenderGo.IntegrationTests
{
    /// <summary>
    /// Positive integration tests for <see cref="UnisenderGoEmailSender"/>.
    /// </summary>
    public class UnisenderGoEmailSender_Should : IClassFixture<UnisenderGoEmailSenderTestFixture>
    {
        private readonly UnisenderGoEmailSenderTestFixture _fixture;
        private readonly ILogger<UnisenderGoEmailSender> _mockLogger;

        /// <inheritdoc cref="UnisenderGoEmailSender_Should"/>
        public UnisenderGoEmailSender_Should(UnisenderGoEmailSenderTestFixture fixture)
        {
            _fixture = fixture ?? throw new ArgumentNullException(nameof(fixture));
            _mockLogger = Mock.Of<ILogger<UnisenderGoEmailSender>>();
        }

        // [Fact]
        [Fact(Skip = "Only for manual testing")]
        public async Task SendEmail_WithoutErrors()
        {
            // arrange
            var sender = new UnisenderGoEmailSender(_mockLogger, _fixture.UnisenderGoEmailOptions);

            // act
            var result = await sender.SendAsync("mm@siisltd.ru", "Hello from Curiosity!", "This is integration test of UnisenderGoEmailSender. We test passing unsubscribe url, tracking reads and links.", true);

            // assert
            result.IsSuccess.Should().BeTrue(result.Errors.FirstOrDefault()?.Description ?? "unknown");
        }
    }
}
