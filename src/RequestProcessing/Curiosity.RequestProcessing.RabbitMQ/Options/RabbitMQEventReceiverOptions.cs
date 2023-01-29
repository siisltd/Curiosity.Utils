using System;
using System.Collections.Generic;
using Curiosity.Configuration;

namespace Curiosity.RequestProcessing.RabbitMQ.Options;

/// <summary>
/// Options for <see cref="RabbitMQEventReceiver"/>.
/// </summary>
public class RabbitMQEventReceiverOptions : ILoggableOptions, IValidatableOptions
{
    /// <summary>
    /// Host where RabbitMQ located.
    /// </summary>
    public string HostName { get; set; } = "localhost";

    /// <summary>
    /// Rabbit's user name.
    /// </summary>
    public string UserName { get; set; } = null!;

    /// <summary>
    /// Rabbit's password name.
    /// </summary>
    public string Password { get; set; } = null!;

    /// <summary>
    /// Rabbit's port.
    /// </summary>
    public int Port { get; set; } = 5672;

    /// <summary>
    /// Name of exchange.
    /// </summary>
    public string ExchangeName { get; set; } = "";

    /// <summary>
    /// Name of RabbitMQ client host name.
    /// </summary>
    public string ClientName { get; set; } = null!;

    /// <summary>
    /// Name of queue to receive events.
    /// </summary>
    public string QueueName { get; set; } = null!;

    /// <inheritdoc />
    public IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
    {
        var errors = new ConfigurationValidationErrorCollection(prefix);

        errors.AddErrorIf(String.IsNullOrEmpty(HostName), nameof(HostName), "can't be empty");
        errors.AddErrorIf(String.IsNullOrEmpty(UserName), nameof(UserName), "can't be empty");
        errors.AddErrorIf(String.IsNullOrEmpty(Password), nameof(Password), "can't be empty");
        errors.AddErrorIf(String.IsNullOrEmpty(ClientName), nameof(ClientName), "can't be empty");
        errors.AddErrorIf(Port < 1, nameof(Port), "can't be less than 1");
        errors.AddErrorIf(String.IsNullOrEmpty(QueueName), nameof(QueueName), "can't be empty");
        errors.AddErrorIf(ExchangeName == null!, nameof(ExchangeName), "can't be null");

        return errors;
    }
}
