using System;

namespace Curiosity.RabbitMQ;

/// <summary>
/// Exception thrown by <see cref="RabbitMqRpcClient"/> when request should be re-queued.
/// </summary>
internal class RabbitMqRpcReQueueException : Exception
{
}
