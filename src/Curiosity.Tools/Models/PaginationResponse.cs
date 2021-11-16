using System;
using System.Collections.Generic;

namespace Curiosity.Tools
{
    /// <summary>
    /// Response contains page body and total count of items
    /// </summary>
    public class PaginationResponse<T> : Response<IReadOnlyList<T>>
    {
        /// <summary>
        /// Current index of the page.
        /// </summary>
        public int PageIndex { get; }

        /// <summary>
        /// Size of the page.
        /// </summary>
        public int PageSize { get; }

        /// <summary>
        /// Total count of items from collection/database.
        /// </summary>
        public int TotalCount { get; }

        protected PaginationResponse(IReadOnlyList<T> body, int pageIndex, int pageSize, int totalCount, bool isSuccess, IReadOnlyList<Error> errors) : base(body, isSuccess, errors)
        {
            PageIndex = pageIndex;
            PageSize = pageSize;
            TotalCount = totalCount;
        }

        /// <summary>
        /// Creates response for successful case.
        /// </summary>
        public static PaginationResponse<T> Successful(Page<T> page)
        {
            if (page == null) throw new ArgumentNullException(nameof(page));

            return new PaginationResponse<T>(page.Data, page.PageIndex, page.PageSize, page.TotalCount, true, Array.Empty<Error>());
        }

        /// <summary>
        /// Creates response for successful case.
        /// </summary>
        public static PaginationResponse<T> Successful(IReadOnlyList<T> body, int pageIndex, int pageSize, int totalCount)
        {
            if (body == null) throw new ArgumentNullException(nameof(body));

            return new PaginationResponse<T>(body, pageIndex, pageSize, totalCount, true, Array.Empty<Error>());
        }

        /// <summary>
        /// Creates response for failed case.
        /// </summary>
        public new static PaginationResponse<T> Failed(IReadOnlyList<Error> errors)
        {
            if (errors == null) throw new ArgumentNullException(nameof(errors));

            return new PaginationResponse<T>(Array.Empty<T>(), 0, 0, 0, false, errors);
        }

        /// <summary>
        /// Creates response for failed case.
        /// </summary>
        public new static PaginationResponse<T> Failed(Error error)
        {
            if (error == null) throw new ArgumentNullException(nameof(error));

            return new PaginationResponse<T>(Array.Empty<T>(), 0, 0, 0, false, new[] { error });
        }
    }
}
