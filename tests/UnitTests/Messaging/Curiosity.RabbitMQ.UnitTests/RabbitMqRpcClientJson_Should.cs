using System;
using System.Text.Json;
using Curiosity.RabbitMQ.RPC;
using FluentAssertions;
using Xunit;

namespace Curiosity.RabbitMQ.UnitTests
{
    /// <summary>
    /// Unit tests for <see cref="RabbitMqRpcClient.DefaultJsonSerializerOptions"/>.
    /// Checks compatibility with messages produced and consumed by Newtonsoft.Json-based RPC peers.
    /// </summary>
    public class RabbitMqRpcClientJson_Should
    {
        public class RpcMessage
        {
            public long Id { get; set; }

            public string? Name { get; set; }

            public decimal Amount { get; set; }

#pragma warning disable CS0649
            public string? Comment;
#pragma warning restore CS0649
        }

        [Fact]
        public void Deserialize_MessageWithDifferentCasingAndNumbersAsStrings()
        {
            // arrange
            var json = "{\"id\":\"42\",\"name\":\"Заявка\",\"amount\":\"10.5\",\"comment\":\"срочно\"}";

            // act
            var message = JsonSerializer.Deserialize<RpcMessage>(json, RabbitMqRpcClient.DefaultJsonSerializerOptions);

            // assert
            message.Should().NotBeNull();
            message!.Id.Should().Be(42);
            message.Name.Should().Be("Заявка");
            message.Amount.Should().Be(10.5m);
            message.Comment.Should().Be("срочно");
        }

        [Fact]
        public void Serialize_MessageLikeNewtonsoft()
        {
            // arrange
            var message = new RpcMessage
            {
                Id = 42,
                Name = "Заявка",
                Amount = 10.5m,
                Comment = "срочно"
            };

            // act
            var json = JsonSerializer.Serialize(message, RabbitMqRpcClient.DefaultJsonSerializerOptions);

            // assert
            json.Should().Be("{\"Id\":42,\"Name\":\"Заявка\",\"Amount\":10.5,\"Comment\":\"срочно\"}");
        }

        [Fact]
        public void BeReadOnly()
        {
            // act
            var act = () => RabbitMqRpcClient.DefaultJsonSerializerOptions.WriteIndented = true;

            // assert
            act.Should().Throw<InvalidOperationException>();
        }
    }
}
