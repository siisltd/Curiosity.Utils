using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.Tools;
using Microsoft.EntityFrameworkCore;

namespace Curiosity.DAL.EF
{
    /// <summary>
    /// Helps to create page from <see cref="IQueryable{T}"/>.
    /// </summary>
    public static class PaginationHelper
    {
        /// <summary>
        /// Return page from specified <see cref="IQueryable{T}"/>.
        /// </summary>
        /// <param name="query">Query to fetch data.</param>
        /// <param name="pageIndex">Current index of the page.</param>
        /// <param name="pageSize">Size of page to fetch.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <typeparam name="T">Type of data.</typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        /// <remarks>
        /// This method also calls Count for <paramref name="query"/>.
        /// </remarks>
        public static async Task<Page<T>> ToPageAsync<T>(this IQueryable<T> query, int pageIndex, int pageSize, CancellationToken cancellationToken = default) where T : class
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            if (pageIndex < 0) throw new ArgumentOutOfRangeException(nameof(pageIndex));
            if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize));

            var totalCount = await query.CountAsync(cancellationToken);

            return await ToPageAsync(query, pageIndex, pageSize, totalCount, cancellationToken);
        }


        /// <summary>
        /// Return page from specified <see cref="IQueryable{T}"/>.
        /// </summary>
        /// <param name="query">Query to fetch data.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">Size of page to fetch.</param>
        /// <param name="totalCount">Total count of items.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <typeparam name="T">Type of data.</typeparam>
        /// <returns></returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static async Task<Page<T>> ToPageAsync<T>(this IQueryable<T> query, int pageIndex, int pageSize, int totalCount, CancellationToken cancellationToken = default) where T : class
        {
            if (query == null) throw new ArgumentNullException(nameof(query));

            if (pageIndex < 0) throw new ArgumentOutOfRangeException(nameof(pageIndex));
            if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize));
            if (totalCount < 0) throw new ArgumentOutOfRangeException(nameof(totalCount));

            var result = await query
                .Skip(pageSize * pageIndex)
                .Take(pageSize)
                .ToArrayAsync(cancellationToken);

            return new Page<T>(pageIndex, pageSize, totalCount, result);
        }
    }
}
