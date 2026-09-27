using Curiosity.SMS.Smsc;
using FluentAssertions;
using RestSharp;
using RestSharp.Serializers.Json;
using Xunit;

namespace Curiosity.SMS.UnitTests
{
    /// <summary>
    /// Unit tests for <see cref="SmscResponseData"/> JSON contract.
    /// </summary>
    public class SmscResponseData_Should
    {
        [Fact]
        public void Deserialize_SuccessResponse_WithRestSharpDefaultSerializer()
        {
            // arrange
            var json = "{\"id\":123,\"cnt\":2,\"cost\":\"3.40\",\"balance\":\"100.50\"}";

            // act
            var data = Deserialize(json);

            // assert
            data.Should().NotBeNull();
            data!.Id.Should().Be(123);
            data.Count.Should().Be(2);
            data.Cost.Should().Be(3.40m);
            data.Balance.Should().Be(100.50m);
            data.Error.Should().BeNull();
            data.error_code.Should().BeNull();
        }

        [Fact]
        public void Deserialize_ErrorResponse_WithRestSharpDefaultSerializer()
        {
            // arrange
            var json = "{\"error\":\"invalid number\",\"error_code\":7}";

            // act
            var data = Deserialize(json);

            // assert
            data.Should().NotBeNull();
            data!.Error.Should().Be("invalid number");
            data.error_code.Should().Be(7);
        }

        [Fact]
        public void Serialize_ResultJson_KeepingPreviousFormat()
        {
            // arrange
            var data = new SmscResponseData
            {
                Error = "Ошибка",
                error_code = -1
            };

            // act
            var json = System.Text.Json.JsonSerializer.Serialize(data, SmscSender.ResultJsonOptions);

            // assert
            json.Should().Be("{\"Error\":\"Ошибка\",\"error_code\":-1}");
        }

        private static SmscResponseData? Deserialize(string json)
        {
            var response = new RestResponse(new RestRequest())
            {
                Content = json,
                ContentType = ContentType.Json
            };

            return new SystemTextJsonSerializer().Deserialize<SmscResponseData>(response);
        }
    }
}
