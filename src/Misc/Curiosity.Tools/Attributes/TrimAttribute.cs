using System;

namespace Curiosity.Tools.Attributes
{
    /// <summary>
    /// Attribute for trimming strings. 
    /// </summary>
    /// <remarks>
    /// To make it work, in ASP.NET in the application, you need to add the model binding
    /// (binders are placed at WebTools.Attributes).
    /// </remarks>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field)]
    public class TrimStringAttribute : Attribute
    {
    }
}