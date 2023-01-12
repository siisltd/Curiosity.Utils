using System;
using System.Collections.Generic;

namespace Curiosity.Tools.Web.Pagination
{
    /// <summary>
    /// 
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Page<T>
    {
        public IReadOnlyCollection<T> Data { get; }
        
        public Paginator Paginator { get; }

        public Page(IReadOnlyCollection<T> data, Paginator paginator)
        {
            Data = data ?? throw new ArgumentNullException(nameof(data));

            Paginator = paginator;
        }
    }
}