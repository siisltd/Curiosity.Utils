using Curiosity.Configuration;
using Curiosity.Hosting;
using Curiosity.RequestProcessing.RabbitMQ.Sample.ConsumerApp.RequestProcessing;
using Curiosity.Tools;

namespace Curiosity.RequestProcessing.RabbitMQ.Sample.ConsumerApp.Configuration;

/// <summary>
/// Configuration of a sample consumer app.
/// </summary>
public class SampleConsumerAppConfiguration : CuriosityAppConfiguration
{
    /// <summary>
    /// Options of request processor.
    /// </summary>
    public SampleRequestProcessorNodeOptions RequestProcessor { get; } = new();

    /// <inheritdoc cref="SampleConsumerAppConfiguration"/>
    public SampleConsumerAppConfiguration()
    {
        AppName = "RabbitMQc Consumer Sample App";
        Culture = new CultureOptions
        {
            DefaultCulture = "en"
        };
    }

    /// <inheritdoc />
    public override IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
    {
        var errors = new ConfigurationValidationErrorCollection(prefix);

        errors.AddErrors(base.Validate(prefix));
        errors.AddErrors(RequestProcessor.Validate(nameof(RequestProcessor)));

        return errors;
    }
}
