using System.Collections.Generic;
using System.Linq;

namespace Curiosity.Tools.Collections
{
    /// <summary>
    /// Extensions for <see cref="IReadOnlyList{T}"/>
    /// </summary>
    public static class IReadOnlyListExtensions
    {
        /// <summary>
        /// Compares whether one list contains the same elements as the second.
        /// </summary>
        /// <remarks>
        /// Source: https://stackoverflow.com/a/3670089/9341757
        /// On changes also update <see cref="ICollectionExtensions"/>.
        /// </remarks>
        public static bool IsScrambledEquals<T>(this IReadOnlyList<T>? list1, IReadOnlyList<T>? list2)
        {
            if (list1 == null || list2 == null) return false;
            if (list1.Count != list2.Count) return false;

            var cnt = new Dictionary<T, int>(list1.Count);

            for (var i = 0; i < list1.Count; i++)
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

            for (var i = 0; i < list2.Count; i++)
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
