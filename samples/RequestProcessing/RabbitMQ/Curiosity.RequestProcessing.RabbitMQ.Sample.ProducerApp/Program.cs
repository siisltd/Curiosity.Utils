// See https://aka.ms/new-console-template for more information

using Curiosity.RequestProcessing.RabbitMQ.Sample.ProducerApp.Startup;

var bootstrapper = new SampleProducerAppBootstrapper();
return await bootstrapper.RunAsync(args);
