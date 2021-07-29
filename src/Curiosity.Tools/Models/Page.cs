using System;
using System.Collections.Generic;

namespace Curiosity.Tools
{
    /// <summary>
    /// Page of some collection.
    /// </summary>
    /// <typeparam name="T">Type of element.</typeparam>
    public class Page<T>
    {
        /// <summary>
        /// Current number of page.
        /// </summary>
        public int PageIndex { get; }

        /// <summary>
        /// Page size.
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// Total count of items in collection.
        /// </summary>
        public int TotalCount { get; }

        /// <summary>
        /// Items.
        /// </summary>
        public IReadOnlyList<T> Data { get; }

        public Page(
            int pageIndex,
            int pageSize,
            int totalCount,
            IReadOnlyList<T>? data)
        {
            if (pageIndex < 0) throw new ArgumentOutOfRangeException(nameof(pageIndex));
            if (pageSize < 0) throw new ArgumentOutOfRangeException(nameof(pageSize));
            if (totalCount < 0) throw new ArgumentOutOfRangeException(nameof(totalCount));

            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = totalCount;
            Data = data ?? Array.Empty<T>();
        }
    }
}
