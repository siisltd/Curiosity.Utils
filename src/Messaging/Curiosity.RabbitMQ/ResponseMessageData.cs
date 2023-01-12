using System;

namespace Curiosity.RabbitMQ;

/// <summary>
/// Response data for <see cref="ResponseMessage"/>.
/// </summary>
/// <remarks>
/// Auxiliary structure for robust and reliable interchange process.
/// </remarks>
public readonly struct ResponseMessageData
{
    /// <summary>
    /// Serialised response data.
    /// </summary>
    public string MessageBody { get; }

    /// <summary>
    /// Name of a queue that will be used for publishing.
    /// </summary>
    public string QueueName { get; }

    /// <inheritdoc cref="ResponseMessageData"/>
    public ResponseMessageData(
        string responseMessage,
        string queueName)
    {
        MessageBody = responseMessage ?? throw new ArgumentNullException(nameof(responseMessage));
        QueueName = queueName ?? throw new ArgumentNullException(nameof(queueName));
    }
}
