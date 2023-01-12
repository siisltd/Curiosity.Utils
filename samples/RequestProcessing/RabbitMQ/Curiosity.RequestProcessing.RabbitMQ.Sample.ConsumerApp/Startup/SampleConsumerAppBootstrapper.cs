using Curiosity.Hosting;
using Curiosity.RequestProcessing.RabbitMQ.Sample.Common;
using Curiosity.RequestProcessing.RabbitMQ;
using Curiosity.RequestProcessing.RabbitMQ.Sample.ConsumerApp.Configuration;
using Curiosity.RequestProcessing.RabbitMQ.Sample.ConsumerApp.RequestProcessing;
using Curiosity.RequestProcessing.Workers;
using Microsoft.Extensions.DependencyInjection;

namespace Curiosity.RequestProcessing.RabbitMQ.Sample.ConsumerApp.Startup;

/// <summary>
/// Class for bootstrapping sample consumer app.
/// </summary>
public class SampleConsumerAppBootstrapper : CuriosityServiceAppBootstrapper<CuriosityCLIArguments, SampleConsumerAppConfiguration>
{
    /// <inheritdoc cref="SampleConsumerAppBootstrapper"/>
    public SampleConsumerAppBootstrapper()
    {
        // register services
        ConfigureServices((_, services, appConfiguration) =>
        {
            // register request processing
            services.AddRabbitMQRequestProcessor<
                SampleRequest,
                SampleRequestProcessingWorker,
                WorkerBasicExtraParams,
                SampleRequestProcessingBootstrapper,
                SampleRequestProcessorNodeOptions,
                SampleRequestDispatcher,
                SampleRequestProcessingInfo>(appConfiguration.RequestProcessor);

            // register metrics collector
            services.AddSingleton<SampleRequestProcessingMetricsCollector>();
            services.AddHostedService(x => x.GetRequiredService<SampleRequestProcessingMetricsCollector>());
        });
    }
}
