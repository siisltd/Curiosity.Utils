using System;
using Curiosity.Configuration;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;

namespace Curiosity.RabbitMQ.RPC;

/// <summary>
/// Factory to creates new instance of <see cref="RabbitMqRpcClient"/>.
/// </summary>
public class RabbitMqRpcClientFactory
{
    private static readonly TimeSpan NetworkRecoveryInterval = TimeSpan.FromSeconds(10);

    private readonly ConnectionFactory _connectionFactory;

    private readonly ILoggerFactory _loggerFactory;
    private readonly RabbitMQOptions _options;
    private readonly ILogger _logger;

    /// <inheritdoc cref="RabbitMqRpcClientFactory"/>
    public RabbitMqRpcClientFactory(
        ILoggerFactory loggerFactory,
        RabbitMQOptions options)
    {
        _loggerFactory = loggerFactory ?? throw new ArgumentNullException(nameof(loggerFactory));
        _options = options ?? throw new ArgumentNullException(nameof(options));

        _options.AssertValid();

        _logger = loggerFactory.CreateLogger<RabbitMqRpcClientFactory>();

        _connectionFactory = new ConnectionFactory
        {
            HostName = options.HostName,
            UserName = options.UserName,
            Password = options.Password,
            Port = options.Port,
            ClientProvidedName = $"{options.ClientName}_rpc_client",
            NetworkRecoveryInterval = NetworkRecoveryInterval,
            AutomaticRecoveryEnabled = true,
            TopologyRecoveryEnabled = true,
        };
    }

    /// <summary>
    /// Creates new RPC client connected to specified queue.
    /// </summary>
    /// <param name="requestQueueName">Queue name.</param>
    /// <param name="clientNameSuffix">Extra for client name.</param>
    /// <param name="disposeResponseQueue">Should response queue be deleted after client disposing?</param>
    public RabbitMqRpcClient CreateClient(
        string requestQueueName,
        string? clientNameSuffix = null,
        bool disposeResponseQueue = true)
    {
        if (String.IsNullOrWhiteSpace(requestQueueName)) throw new ArgumentNullException(nameof(requestQueueName));

        var responseQueueName = $"{requestQueueName}_responses_{_options.ClientName}";
        var loggerName = $"RabbitMqRpcClient_{requestQueueName}";
        var clientName = _options.ClientName;
        if (!String.IsNullOrWhiteSpace(clientNameSuffix))
        {
            responseQueueName = $"{responseQueueName}_{clientNameSuffix}";
            loggerName = $"{loggerName}_{clientNameSuffix}";
            clientName = $"{clientName}_{clientNameSuffix}";
        }

        var logger = _loggerFactory.CreateLogger(loggerName);

        var client = new RabbitMqRpcClient(
            clientName,
            _options.ExchangeName,
            requestQueueName,
            responseQueueName,
            logger,
            NetworkRecoveryInterval,
            _connectionFactory.CreateConnection,
            disposeResponseQueue);

        client.Init();

        _logger.LogDebug(
            "Created new RPC client and made it connected to RabbitMQ (host \"{RabbitMqHostName}\", queue = \"{RequestQueueName}\", response queue = \"{ResponseQueueName}\")",
            _options.HostName,
            requestQueueName,
            responseQueueName);

        return client;
    }
}
