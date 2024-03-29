using System.Collections.Generic;
using System.Linq;

namespace Curiosity.Tools.Collections
{
    /// <summary>
    /// Extensions for <see cref="ICollection{T}"/>
    /// </summary>
    public static class ICollectionExtensions
    {
        /// <summary>
        /// Compares whether one list contains the same elements as the second.
        /// </summary>
        /// <remarks>
        /// Source: https://stackoverflow.com/a/3670089/9341757
        /// On changes also update <see cref="IReadOnlyListExtensions"/>.
        /// </remarks>
        public static bool IsScrambledEquals<T>(this ICollection<T>? list1, IReadOnlyCollection<T>? list2)
        {
            if (list1 == null || list2 == null) return false;
            if (list1.Count != list2.Count) return false;

            var cnt = new Dictionary<T, int>(list1.Count);
            
            foreach (var item in list1)
            {
                if (cnt.ContainsKey(item))
                {
                    cnt[item]++;
                }
                else
                {
                    cnt.Add(item, 1);
                }
            }

            foreach (var item in list2)
            {
                if (cnt.ContainsKey(item))
                {
                    cnt[item]--;
                }
                else
                {
                    return false;
                }
            }
            
            return cnt.Values.All(c => c == 0);
        }
        
        public static bool IsScrambledEquals<T>(this IList<T>? list1, IList<T>? list2)
        {
            if (list1 == null || list2 == null) return false;
            if (list1.Count != list2.Count) return false;

            var cnt = new Dictionary<T, int>(list1.Count);

            for (int i = 0; i < list1.Count; i++)
            {
                var item = list1[i];
                if (cnt.ContainsKey(item))
                {
                    cnt[item]++;
                }
                else
                {
                    cnt.Add(item, 1);
                } 
            }

            for (int i = 0; i < list2.Count; i++)
            {
                var item = list2[i];
                if (cnt.ContainsKey(item))
                {
                    cnt[item]--;
                }
                else
                {
                    return false;
                }
            }
           
            return cnt.Values.All(c => c == 0);
        }
    }
}
