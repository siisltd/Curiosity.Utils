using System;
using System.Globalization;

namespace Curiosity.RequestProcessing.RabbitMQ;

/// <summary>
/// Wrapper of request from RabbitMQ. 
/// </summary>
/// <remarks>
/// Contains additional methods and properties that helps process request. 
/// </remarks>
public class RabbitMQRequestWrapper<T> : IRequest
{
    /// <inheritdoc />
    public long Id { get; }

    /// <inheritdoc />
    public CultureInfo RequestCulture { get; }

    /// <summary>
    /// Correlation id of a request.
    /// </summary>
    public string CorrelationId => RabbitMQEvent.ReceivedData.BasicProperties.CorrelationId;

    /// <summary>
    /// Source request from RabbitMQ.
    /// </summary>
    public T SourceRequest { get; }

    /// <summary>
    /// Event data from RabbitMQ.
    /// </summary>
    /// <remarks>
    /// Uses for confirming or rejecting the request.
    /// </remarks>
    protected RabbitMQEvent RabbitMQEvent { get; }

    /// <inheritdoc cref="RabbitMQRequestWrapper{T}"/>
    public RabbitMQRequestWrapper(
        long id,
        CultureInfo requestCulture,
        T sourceRequest,
        RabbitMQEvent rabbitMQEvent)
    {
        if (id < 0) throw new ArgumentOutOfRangeException(nameof(id));

        Id = id;
        RequestCulture = requestCulture ?? throw new ArgumentNullException(nameof(requestCulture));
        SourceRequest = sourceRequest;
        RabbitMQEvent = rabbitMQEvent ?? throw new ArgumentNullException(nameof(rabbitMQEvent));
    }
    
    /// <summary>
    /// Confirms processing of a request.
    /// </summary>
    public void ConfirmProcessing()
    {
        RabbitMQEvent.ConfirmProcessing();
    }

    /// <summary>
    /// Rejects processing of a request.
    /// </summary>
    public void RejectProcessing()
    {
        RabbitMQEvent.RejectProcessing();
    }
}
