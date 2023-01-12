using Curiosity.Configuration;
using Curiosity.Hosting;
using Curiosity.RabbitMQ;

namespace Curiosity.RequestProcessing.RabbitMQ.Sample.ProducerApp.Configuration;

/// <summary>
/// Options for sample producer app.
/// </summary>
public class SampleProducerAppConfiguration : CuriosityAppConfiguration
{
    /// <summary>
    /// RabbitMQ options.
    /// </summary>
    public RabbitMQOptions RabbitMQ { get; } = new();

    /// <summary>
    /// Name of RabbitMQ queue to send requests.
    /// </summary>
    public string QueueName { get; set; } = null!;

    /// <summary>
    /// Delay between sending requests, ms.
    /// </summary>
    public int DelayBetweenSendMs { get; set; } = 10;

    /// <inheritdoc />
    public SampleProducerAppConfiguration()
    {
        AppName = "RabbitMQc Consumer Producer App";
    }

    /// <inheritdoc />
    public override IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
    {
        var errors = new ConfigurationValidationErrorCollection(prefix);
        errors.AddErrors(base.Validate(prefix));
        errors.AddErrorIf(String.IsNullOrWhiteSpace(QueueName), nameof(QueueName), "can't be empty");
        errors.AddErrorIf(DelayBetweenSendMs < 1, nameof(DelayBetweenSendMs), "can't be less than 1");
        errors.AddErrors(RabbitMQ.Validate());
        return errors;
    }
}
