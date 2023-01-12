using Curiosity.Hosting;
using Curiosity.RequestProcessing.RabbitMQ.Sample.ProducerApp.Configuration;
using Curiosity.RequestProcessing.RabbitMQ.Sample.ProducerApp.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Curiosity.RequestProcessing.RabbitMQ.Sample.ProducerApp.Startup;

/// <summary>
/// Class for bootstrapping sample producer app.
/// </summary>
public class SampleProducerAppBootstrapper : CuriosityServiceAppBootstrapper<CuriosityCLIArguments, SampleProducerAppConfiguration>
{
    /// <inheritdoc cref="SampleRequestsProducer"/>
    public SampleProducerAppBootstrapper()
    {
        ConfigureServices((_, services, _) =>
        {
            services.AddHostedService<SampleRequestsProducer>();
        });
    } 
}
