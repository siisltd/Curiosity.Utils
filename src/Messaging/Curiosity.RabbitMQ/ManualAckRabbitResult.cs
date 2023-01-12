using System;

namespace Curiosity.RabbitMQ;

/// <summary>
/// Result of executing RPC via Rabbit that requires manual acknowledge confirmation.
/// </summary>
/// <typeparam name="T"></typeparam>
public class ManualAckRabbitResult<T>
{
    private readonly Action _confirmationAction;

    private bool _haveAlreadyConfirmed;

    /// <summary>
    /// Result of RPC call.
    /// </summary>
    public T Data { get; }

    /// <inheritdoc cref="ManualAckRabbitResult{T}"/>
    internal ManualAckRabbitResult(T data, Action confirmationAction)
    {
        Data = data ?? throw new ArgumentNullException(nameof(data));
        _confirmationAction = confirmationAction ?? throw new ArgumentNullException(nameof(confirmationAction));
        _haveAlreadyConfirmed = false;
    }

    /// <summary>
    /// Confirms processing of <see cref="Data"/>.
    /// </summary>
    /// <remarks>
    /// Sends ack to RabbitMQ to remove result from response queue.
    /// </remarks>
    public void ConfirmAcknowledge()
    {
        if (_haveAlreadyConfirmed) return;

        _confirmationAction.Invoke();
        _haveAlreadyConfirmed = true;
    }
}
