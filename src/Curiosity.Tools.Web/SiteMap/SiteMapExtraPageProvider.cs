using System;
using System.Collections.Generic;

namespace Curiosity.Tools.Web.SiteMap
{
    /// <summary>
    /// Provides extra pages for site map.
    /// </summary>
    public class SiteMapExtraPageProvider
    {
        /// <summary>
        /// Provides extra pages for site map.
        /// </summary>
        public virtual IReadOnlyList<(string url, DateTime lastChangeData)> GetExtraPages()
        {
            return Array.Empty<(string, DateTime)>();
        }
    }
}