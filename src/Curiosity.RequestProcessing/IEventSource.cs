using System;

namespace Curiosity.RequestProcessing
{
    public interface IEventSource
    {

    }

    public interface IEventSource<T> : IEventSource, IEquatable<T> where T: class
    {

    }
}
