using System;
using System.Collections.Generic;
using System.Threading;

namespace Curiosity.Tools.Collections
{
    /// <summary>
    /// Extensions for <see cref="IList{T}"/>
    /// </summary>
    public static class IListExtensions
    {
        /// <summary>
        /// Seed value for random instance
        /// </summary>
        private static int _randomSeed = Environment.TickCount;

        /// <summary>
        /// Random instance 
        /// </summary>
        /// <remarks>
        /// ThreadLocal allows us to make a different version of static variable for each thread.
        /// Since we usually use a thread pool redundant instances would not be created.
        /// ThreadLocal allows us not to do lock, while being faster and thread-safe
        /// </remarks>
        private static readonly ThreadLocal<Random> Random = new ThreadLocal<Random>(() => new Random(Interlocked.Increment(ref _randomSeed)));

        public static void Shuffle<T>(this IList<T> list)  
        {
            if (list == null) throw new ArgumentNullException(nameof(list));
            
            var n = list.Count;  
            while (n > 1) 
            {  
                n--;  
                var k = Random.Value.Next(n + 1);  
                var value = list[k];  
                list[k] = list[n];  
                list[n] = value;  
            }  
        }
        
        public static IList<T> GetRandomItems<T>(this IList<T> list, int count)
        {
            if (count <= 0) throw new ArgumentOutOfRangeException(nameof(count));
            if (list.Count == 0) return list;

            var desiredCount = Math.Min(count, list.Count);

            // if size is small, just shuffle the collection
            if (desiredCount == list.Count)
            {
                var copy = new List<T>(list);
                copy.Shuffle();
                return copy;
            }
            
            var unusedItemIds = new List<int>(list.Count);
            
            for (var i=0; i< list.Count; ++i)
            {
                unusedItemIds.Add(i);
            }
            
            var results = new List<T>(desiredCount);

            do
            {
                // select random item id
                var randomIndex = Random.Value.Next(0, unusedItemIds.Count - 1);
                var itemIndex = unusedItemIds[randomIndex];
                
                // update unused items
                unusedItemIds.RemoveAt(randomIndex);

                var post = list[itemIndex];
                results.Add(post);
            }
            while (results.Count != desiredCount && unusedItemIds.Count > 0);
            
            return results;
        }
        
        /// <summary>
        /// Randomizes list.
        /// </summary>
        public static void RandomizeList<T>(this IList<T> list)
        {
            Random random = new Random();
            int n = list.Count;
            while (n-- > 1)
            {
                int k = random.Next(n + 1);

                var value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        /// <summary>
        /// Rotates list.
        /// </summary>
        public static void RotateList<T>(this IList<T> list, int shift)
        {
            var count = list.Count;
            if (count <= 1) return;
            
            shift = shift % count;
            while (shift-- > 0)
            {
                var last = list[count - 1];
                for (int i = count - 1; i > 0; i--)
                {
                    list[i] = list[i - 1];
                }
                list[0] = last;
            }
        }
    }
}