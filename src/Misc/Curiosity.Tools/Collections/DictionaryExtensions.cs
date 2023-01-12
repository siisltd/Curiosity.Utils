using System;
using System.Collections.Generic;

namespace Curiosity.Tools.Collections
{
    /// <summary>
    /// Extensions for <see cref="Dictionary{TKey,TValue}"/>.
    /// </summary>
    public static class DictionaryExtensions
    {
        /// <summary>
        /// Updates one dictionary from another.
        /// </summary>
        public static Dictionary<string, T>? UpdateFrom<T>(
            this Dictionary<string, T>? destination,
            Dictionary<string, T>? source,
            StringComparer? comparer)
        {
            if (source == null)
                return null;

            if (destination != null && !Equals(destination.Comparer, comparer))
            {
                destination = null;
            }
            if (destination != null)
            {
                destination.Clear();
            }
            else
            {
                destination = new Dictionary<string, T>(comparer);
            }

            foreach (var item in source)
            {
                if (String.IsNullOrEmpty(item.Key)) continue;
                
                if (destination.ContainsKey(item.Key))
                {
                    destination[item.Key] = item.Value;
                }
                else
                {
                    destination.Add(item.Key, item.Value);
                }
            }

            return destination;
        }
    }
}