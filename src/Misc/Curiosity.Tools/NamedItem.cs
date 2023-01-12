using System;

namespace Curiosity.Tools
{
    public class NamedItem<TId>
    {
        public TId Id { get; }
        public string Name { get; }

        public NamedItem(TId id, string name)
        {
            Id = id ?? throw new ArgumentNullException(nameof(id));
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }
    }
}