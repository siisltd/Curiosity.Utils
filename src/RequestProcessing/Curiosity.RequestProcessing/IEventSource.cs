using System;

namespace Curiosity.RequestProcessing
{
    /// <summary>
    /// Интерфейс-маркер для источника событий для обработчика запросов.
    /// </summary>
    public interface IEventSource
    {
    }

    /// <summary>
    /// <inheritdoc cref="IEventSource"/>
    /// </summary>
    public interface IEventSource<T> : IEventSource, IEquatable<T> where T: class
    {
    }
}
