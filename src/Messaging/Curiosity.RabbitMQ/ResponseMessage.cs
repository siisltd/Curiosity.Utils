namespace Curiosity.RabbitMQ;

/// <summary>
/// Data for sending to RabbitMQ as a response for some request.
/// </summary>
/// <remarks>
/// In most cases it is response data for some kind of a request.
/// Auxiliary structure for robust and reliable interchange process.
/// </remarks>
public readonly struct ResponseMessage
{
    /// <summary>
    /// Delivery tag of a request.
    /// </summary>
    /// <remarks>
    /// Uses for ack/nack of a request.
    /// </remarks>
    public ulong RequestDeliveryTag { get; private init;}

    /// <summary>
    /// CorrelationId of a request.
    /// </summary>
    public string CorrelationId { get; private init; }

    public ResponseMessageData? ResponseData { get; private init; }

    /// <summary>
    /// Creates new instance of <see cref="ResponseMessage"/> for data publish.
    /// </summary>
    public static ResponseMessage CreateForPublish(
        string responseMessage,
        string correlationId,
        string queueName,
        ulong deliveryTag)
    {
        return new ResponseMessage
        {
            CorrelationId = correlationId,
            RequestDeliveryTag = deliveryTag,
            ResponseData = new ResponseMessageData(
                responseMessage,
                queueName)
        };
    }

    /// <summary>
    /// Creates new instance of <see cref="ResponseMessage"/> for data rejection.
    /// </summary>
    public static ResponseMessage CreateForRejection(
        ulong deliveryTag,
        string correlationId)
    {
        return new ResponseMessage
        {
            CorrelationId = correlationId,
            RequestDeliveryTag = deliveryTag,
            ResponseData = null
        };
    }
}
