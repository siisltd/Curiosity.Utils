using System;
using System.Collections.Generic;

namespace Curiosity.Tools.Collections
{
    /// <summary>
    /// Extensions for <see cref="HashSet{T}"/>
    /// </summary>
    public static class HashSetExtensions
    {
        /// <summary>
        /// Add item conditionally.
        /// </summary>
        public static void AddIf<T>(this HashSet<T> hashSet, bool condition, params T[] elements)
        {
            if (hashSet == null) throw new ArgumentNullException(nameof(hashSet));
            
            if (condition)
            {
                hashSet.AddAll(elements);
            }
        }
        
        /// <summary>
        /// Adds all elements to collection.
        /// </summary>
        public static void AddAll<T>(this HashSet<T> hashSet, params T[] elements)
        {
            for (var i = 0; i < elements.Length; i++)
            {
                hashSet.Add(elements[i]);
            }
        }
    }
}