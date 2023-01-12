using Curiosity.Configuration;
using Curiosity.RequestProcessing;
using Curiosity.RequestProcessing.RabbitMQ.Options;

namespace Curiosity.RequestProcessing.RabbitMQ.Sample.ConsumerApp.RequestProcessing;

/// <summary>
/// Options for request processor.
/// </summary>
public class SampleRequestProcessorNodeOptions : RequestProcessorNodeOptions, IRabbitMQRequestProcessorNodeOptions
{
    /// <inheritdoc cref="IRabbitMQRequestProcessorNodeOptions.RabbitMQEventReceiver"/>
    public RabbitMQEventReceiverOptions RabbitMQEventReceiver { get; } = new();

    /// <inheritdoc />
    public override IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
    {
        var errors = new ConfigurationValidationErrorCollection(prefix);
        errors.AddErrors(base.Validate(prefix));
        errors.AddErrors(RabbitMQEventReceiver.Validate(nameof(RabbitMQEventReceiver)));
        return errors;
    }
}
