using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Curiosity.MassEventBus
{

    public class MassEventBusOptions : ILoggableOptions, IValidatableOptions
    {
        public string RabbitHostName { get; set; }

        public string RabbitUserName { get; set; }

        public string RabbitPassword { get; set; }

        public string QueueName { get; set; }

        public string? ClientName { get; set; } = Environment.MachineName;

        /// <inheritdoc />
        public IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
        {
            return Array.Empty<ConfigurationValidationError>();
        }
    }

    public interface IMassEventPayload
    {
    }

    public class MassEvent
    {
        public string EventName { get; }

        public IMassEventPayload? Payload { get; }

        public MassEvent(string eventName, IMassEventPayload? payload)
        {
            if (String.IsNullOrWhiteSpace(eventName)) throw new ArgumentNullException(nameof(eventName));

            EventName = eventName;
            Payload = payload;
        }
    }

    public class MassEventProducer : BackgroundService
    {
        private readonly MassEventBusOptions _options;
        private readonly ILogger _logger;

        private readonly BlockingCollection<MassEvent> _localQueue;

        private IConnection _rabbitConnection = null!;
        private IModel _rabbitChannel = null!;

        public MassEventProducer(
            MassEventBusOptions options,
            ILogger<MassEventProducer> logger)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            options.AssertValid();

            _localQueue = new BlockingCollection<MassEvent>(new ConcurrentQueue<MassEvent>(), 10_000);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();
        }

        public override Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogDebug($"Starting {nameof(MassEventProducer)} (queue=\"{_options.QueueName}\", host=\"{_options.RabbitHostName}\")...");
            var factory = new ConnectionFactory
            {
                HostName = _options.RabbitHostName,
                UserName = _options.RabbitUserName,
                Password = _options.RabbitPassword,
                ClientProvidedName = _options.ClientName ?? Environment.MachineName
            };

            _rabbitConnection = factory.CreateConnection();
            _rabbitChannel = _rabbitConnection.CreateModel();

            _logger.LogInformation($"Starting {nameof(MassEventProducer)} (queue=\"{_options.QueueName}\", host=\"{_options.RabbitHostName}\") completed");

            return base.StartAsync(cancellationToken);
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            await base.StopAsync(cancellationToken);

            _logger.LogDebug($"Stopping {nameof(MassEventProducer)} (queue=\"{_options.QueueName}\", host=\"{_options.RabbitHostName}\")...");

            if (_rabbitChannel != null!)
            {
                _rabbitChannel.Dispose();
                _rabbitChannel = null!;
            }

            if (_rabbitChannel != null!)
            {
                _rabbitConnection.Dispose();
                _rabbitConnection = null!;
            }

            _logger.LogInformation($"Stopping {nameof(MassEventProducer)} (queue=\"{_options.QueueName}\", host=\"{_options.RabbitHostName}\") completed.");
        }
    }

    public class MassEventConsumer
    {

    }
}
