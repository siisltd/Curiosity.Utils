using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        /// <summary>
        /// Get all combinations from specified collection.
        /// </summary>
        /// <param name="items">Collection</param>
        /// <typeparam name="T">Type of item</typeparam>
        /// <returns>All combinations</returns>
        /// <remarks>
        /// On changes also update <see cref="IListExtensions"/>.
        /// </remarks>
        public static IEnumerable<IReadOnlyList<T>> Permute<T>(this IReadOnlyList<T> items)
        {
            foreach (var readOnlyList in Permute(items, 0, new List<T>(items.Count), new bool[items.Count]))
            {
                yield return readOnlyList;
            }
        }

        private static IEnumerable<IReadOnlyList<T>> Permute<T>(IReadOnlyList<T> items, int index, List<T> permutation, bool[] used)
        {
            for (int i = 0; i < items.Count; ++i)
            {
                if (used[i]) continue;

                var newPermutation = new List<T>(permutation);

                used[i] = true;
                newPermutation.Add(items[i]);

                if (index < items.Count - 1)
                {
                    foreach (var result in Permute(items, index + 1, newPermutation, used))
                    {
                        yield return result;
                    }
                }
                else
                {
                    yield return newPermutation;
                }

                used[i] = false;
            }
        }
    }
}
