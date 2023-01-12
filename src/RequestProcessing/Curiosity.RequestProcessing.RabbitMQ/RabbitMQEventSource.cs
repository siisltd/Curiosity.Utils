using System;
using Curiosity.RequestProcessing.RabbitMQ.Options;

namespace Curiosity.RequestProcessing.RabbitMQ;

/// <summary>
/// RabbitMQ event source to listen.
/// </summary>
public class RabbitMQEventSource : IEventSource
{
    /// <summary>
    /// Options to connect to RabbitMQ.
    /// </summary>
    public RabbitMQEventReceiverOptions RabbitMQOptions { get; }

    /// <inheritdoc cref="RabbitMQEventSource"/>
    public RabbitMQEventSource(RabbitMQEventReceiverOptions rabbitMQOptions)
    {
        RabbitMQOptions = rabbitMQOptions ?? throw new ArgumentNullException(nameof(rabbitMQOptions));
    }
}
