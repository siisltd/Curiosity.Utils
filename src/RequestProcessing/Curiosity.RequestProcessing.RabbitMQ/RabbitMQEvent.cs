using System;
using RabbitMQ.Client.Events;

namespace Curiosity.RequestProcessing.RabbitMQ;

/// <summary>
/// Event received from RabbitMQ.
/// </summary>
public class RabbitMQEvent : IRequestProcessingEvent
{
    /// <summary>
    /// Can methods <see cref="ConfirmProcessing"/> or <see cref="RejectProcessing"/> be called.
    /// </summary>
    private bool _canMakeDecision;

    private readonly Action<ulong> _confirmAction;
    private readonly Action<ulong> _rejectAction;

    /// <summary>
    /// Data received from RabbitMQ.
    /// </summary>
    /// <remarks>
    /// Dont' use body property, Use <see cref="Payload"/> instead.
    /// </remarks>
    public BasicDeliverEventArgs ReceivedData { get; }

    /// <summary>
    /// Received payload.
    /// </summary>
    public byte[] Payload { get; }

    /// <inheritdoc cref="RabbitMQEvent"/>
    internal RabbitMQEvent(
        BasicDeliverEventArgs receivedArgs,
        Action<ulong> confirmAction,
        Action<ulong> rejectAction)
    {
        _confirmAction = confirmAction ?? throw new ArgumentNullException(nameof(confirmAction));
        _rejectAction = rejectAction ?? throw new ArgumentNullException(nameof(rejectAction));
        ReceivedData = receivedArgs ?? throw new ArgumentNullException(nameof(receivedArgs));

        // we need to copy because RabbitMQ used under the hood ReadOnlyMemory
        // and after returning from handler method body can be changed
        // https://www.rabbitmq.com/dotnet-api-guide.html#consuming-memory-safety
        Payload = receivedArgs.Body.ToArray();

        _canMakeDecision = true;
    }

    /// <summary>
    /// Confirms processing of event.
    /// </summary>
    public void ConfirmProcessing()
    {
        if (!_canMakeDecision) return;

        _confirmAction(ReceivedData.DeliveryTag);
        _canMakeDecision = false;
    }

    /// <summary>
    /// Rejects processing of event.
    /// </summary>
    public void RejectProcessing()
    {
        if (!_canMakeDecision) return;

        _rejectAction(ReceivedData.DeliveryTag);
        _canMakeDecision = false;
    }
}
