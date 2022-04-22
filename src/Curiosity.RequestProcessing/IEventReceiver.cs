using System;
using Microsoft.Extensions.Hosting;

namespace Curiosity.RequestProcessing
{
    /// <summary>
    /// Получатель событий для старта обработки.
    /// </summary>
    public interface IEventReceiver : IHostedService
    {
        /// <summary>
        /// Получение события от Postgres, на которое мы подписаны.
        /// </summary>
        public event EventHandler<IRequestProcessingEvent>? OnEventReceived;
    }
}
