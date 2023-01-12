using Curiosity.Configuration;

namespace Curiosity.RequestProcessing.RabbitMQ.Options;

/// <summary>
/// Marker for options for processing RabbitMQ requests..
/// </summary>
public interface IRabbitMQRequestProcessorNodeOptions : ILoggableOptions, IValidatableOptions
{
    /// <summary>
    /// Options to configure RabbitMQ events receiver.
    /// </summary>
    RabbitMQEventReceiverOptions RabbitMQEventReceiver { get; }
}
