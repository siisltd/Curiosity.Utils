using System;
using System.Collections.Generic;
using Curiosity.Configuration;

namespace Curiosity.RabbitMQ;

/// <summary>
/// Options to connect to RabbitMQ.
/// </summary>
public class RabbitMQOptions : IValidatableOptions, ILoggableOptions
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
    /// Name of exchange.
    /// </summary>
    public string ExchangeName { get; set; } = "";

    /// <summary>
    /// Name of RabbitMQ client host name.
    /// </summary>
    public string ClientName { get; set; } = $"BWKR_{Environment.MachineName}";

    /// <inheritdoc />
    public IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
    {
        var errors = new ConfigurationValidationErrorCollection(prefix);

        errors.AddErrorIf(String.IsNullOrEmpty(HostName), nameof(HostName), "can't be empty");
        errors.AddErrorIf(String.IsNullOrEmpty(UserName), nameof(UserName), "can't be empty");
        errors.AddErrorIf(String.IsNullOrEmpty(Password), nameof(Password), "can't be empty");
        errors.AddErrorIf(String.IsNullOrEmpty(ClientName), nameof(ClientName), "can't be empty");
        errors.AddErrorIf(ExchangeName == null!, nameof(ExchangeName), "can't be null");

        return errors;
    }
}
