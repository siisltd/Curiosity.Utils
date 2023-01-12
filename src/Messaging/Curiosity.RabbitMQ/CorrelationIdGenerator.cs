using System;
using Curiosity.Configuration;
using Curiosity.Tools;

namespace Curiosity.RabbitMQ;

/// <summary>
/// Generator of unique and readable correlationId.
/// </summary>
public class CorrelationIdGenerator
{
    private readonly RabbitMQOptions _rabbitMqOptions;

    /// <inheritdoc cref="CorrelationIdGenerator"/>
    public CorrelationIdGenerator(RabbitMQOptions rabbitMqOptions)
    {
        _rabbitMqOptions = rabbitMqOptions ?? throw new ArgumentNullException(nameof(rabbitMqOptions));
        _rabbitMqOptions.AssertValid();
    }

    /// <summary>
    /// Generates unique correlation id.
    /// </summary>
    public string GenerateCorrelationId()
    {
        return $"{_rabbitMqOptions.ClientName}_r{UniqueIdGenerator.Generate().ToPublicId()}";
    }

    /// <summary>
    /// Generates unique correlation id using specified existed correlation id as base.
    /// </summary>
    public string GenerateCorrelationId(string baseCorrelationId)
    {
        if (String.IsNullOrWhiteSpace(baseCorrelationId)) throw new ArgumentNullException(nameof(baseCorrelationId));

        return $"{baseCorrelationId}->{_rabbitMqOptions.ClientName}_r{UniqueIdGenerator.Generate().ToPublicId()}";
    }
}
