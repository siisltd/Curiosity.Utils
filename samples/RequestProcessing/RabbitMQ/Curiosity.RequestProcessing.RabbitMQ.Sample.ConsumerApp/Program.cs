using Curiosity.RequestProcessing.RabbitMQ.Sample.ConsumerApp.Startup;

var appBootstrapper = new SampleConsumerAppBootstrapper();
return await appBootstrapper.RunAsync(args);
