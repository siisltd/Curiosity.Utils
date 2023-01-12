using System;
using System.Text;

namespace Curiosity.Tools
{
    /// <summary>
    /// Extension methods for <see cref="StringBuilder"/>
    /// </summary>
    public static class StringBuilderExtensions
    {
        /// <summary>
        /// Append new formatted line 
        /// </summary>
        /// <param name="sb">Instance of <see cref="StringBuilder"/></param>
        /// <param name="fmt">String format</param>
        /// <param name="args">Arguments for string</param>
        /// <returns>Instance of <see cref="StringBuilder"/> with added formatted string</returns>
        public static StringBuilder AppendLine(this StringBuilder sb, string fmt, params object[] args)
        {
            return sb.AppendLine(String.Format(fmt, args));
        }
    }
}