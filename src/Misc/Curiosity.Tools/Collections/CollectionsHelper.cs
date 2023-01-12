using System;
using System.Collections.Generic;
using System.Linq;

namespace Curiosity.Tools.Collections
{
    /// <summary>
    /// Helpers for managing collections.
    /// </summary>
    public static class CollectionsHelper
    {
        /// <summary>
        /// Creates lists for deleting and adding based on the list of new items and existing ones.
        /// </summary>
        public static (IReadOnlyCollection<T> ToAddIds, IReadOnlyCollection<T> ToDeleteIds) GetListForAddingAndDeleting<T>(
            IReadOnlyCollection<T> newItems,
            IReadOnlyCollection<T> existedItems)
        {
            if (newItems == null) throw new ArgumentNullException(nameof(newItems));
            if (existedItems == null) throw new ArgumentNullException(nameof(existedItems));
            var toAddIds = newItems.Except(existedItems).ToList();
            var toDeleteIds = existedItems.Except(newItems).ToList();

            return (toAddIds, toDeleteIds);
        }
        
        /// <summary>
        /// Creates lists for deleting and adding based on the list of new items and existing ones.
        /// </summary>
        public static (ICollection<T> ToAddIds, ICollection<T> ToDeleteIds) GetListForAddingAndDeleting<T>(
            ICollection<T> newItems,
            ICollection<T> existedItems)
        {
            if (newItems == null) throw new ArgumentNullException(nameof(newItems));
            if (existedItems == null) throw new ArgumentNullException(nameof(existedItems));
            
            var toAddIds = newItems.Except(existedItems).ToList();
            var toDeleteIds = existedItems.Except(newItems).ToList();

            return (toAddIds, toDeleteIds);
        }
    }
}