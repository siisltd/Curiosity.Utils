using System;

namespace Curiosity.Tools.Web.SiteMap
{
    /// <summary>
    /// Attribute for avoiding adding action to site map
    /// </summary>
    [AttributeUsage(AttributeTargets.Method)]
    public class SkipSeoIndexing : Attribute
    {
    }
}