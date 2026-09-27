using System.Text.Json;
using Curiosity.Email.UnisenderGo;
using FluentAssertions;
using Xunit;

namespace Curiosity.Emails.UnitTests
{
    /// <summary>
    /// Unit tests for JSON contract of UnisenderGo API DTOs.
    /// </summary>
    public class UnisenderGoSerialization_Should
    {
        [Fact]
        public void Serialize_Request_WithSnakeCaseNamesAndWithoutNulls()
        {
            // arrange
            var request = new UnisenderGoSendEmailRequest
            {
                Message = new UnisenderGoSendEmailMessage
                {
                    Recipients = new[] { new UnisenderGoRecipient { Email = "john@siisltd.ru" } },
                    Body = new UnisenderGoSendEmailMessageBody { Html = "<p>Привет</p>" },
                    Subject = "Тема",
                    FromEmail = "noreply@siisltd.ru",
                    FromName = "SIIS",
                    TrackLinks = 1
                }
            };

            // act
            var json = JsonSerializer.Serialize(request, UnisenderGoEmailSender.SerializerOptions);

            // assert
            json.Should().Be(
                "{\"message\":{\"recipients\":[{\"email\":\"john@siisltd.ru\"}]," +
                "\"body\":{\"html\":\"<p>Привет</p>\"}," +
                "\"subject\":\"Тема\",\"from_email\":\"noreply@siisltd.ru\",\"from_name\":\"SIIS\",\"track_links\":1}}");
        }

        [Fact]
        public void Deserialize_SuccessResponse()
        {
            // arrange
            var json = "{\"status\":\"success\",\"job_id\":\"1ZymBc-00041N-9X\",\"emails\":[\"john@siisltd.ru\"]}";

            // act
            var response = JsonSerializer.Deserialize<UnisenderGoSendEmailResponse>(json, UnisenderGoEmailSender.SerializerOptions);

            // assert
            response.Should().NotBeNull();
            response!.IsSuccessful.Should().BeTrue();
            response.JobId.Should().Be("1ZymBc-00041N-9X");
            response.Emails.Should().Equal("john@siisltd.ru");
        }

        [Fact]
        public void Deserialize_FailedResponse()
        {
            // arrange
            var json = "{\"status\":\"error\",\"code\":204,\"message\":\"No valid recipients\"," +
                       "\"failed_emails\":{\"john@siisltd.ru\":\"permanent_unavailable\"}}";

            // act
            var response = JsonSerializer.Deserialize<UnisenderGoSendEmailResponse>(json, UnisenderGoEmailSender.SerializerOptions);

            // assert
            response.Should().NotBeNull();
            response!.IsSuccessful.Should().BeFalse();
            response.Code.Should().Be(204);
            response.Message.Should().Be("No valid recipients");
            response.FailedEmails.Should().ContainKey("john@siisltd.ru").WhoseValue.Should().Be("permanent_unavailable");
        }
    }
}
