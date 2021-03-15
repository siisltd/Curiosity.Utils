using System;
using Microsoft.Extensions.Logging;

namespace Curiosity.Tools.Performance
{
    /// <summary>
    /// Class for measure duration of a some code. 
    /// </summary>
    public class PerformanceManager
    {
        private static ILogger? _logger;

        /// <summary>
        /// Initializes manager with default values.
        /// </summary>
        public static void Initialize(ILogger? log = null)
        {
            if (log != null)
            {
                _logger = log;
            }
        }

        /// <summary>
        /// Creates a measurer object with specified name.
        /// </summary>
        /// <param name="name">Name for measurer.</param>
        /// <returns>New instance of measurer.</returns>
        public static PerformanceMeasurer Measure(string name)
        {
            return new PerformanceMeasurer(_logger, name);
        }

        /// <summary>
        /// Creates a measurer object with specified name format.
        /// </summary>
        /// <param name="name">Name format for measurer.</param>
        /// <param name="args">Arguments for format</param>
        /// <returns>New instance of measurer.</returns>
        public static PerformanceMeasurer Measure(string name, params object[]? args)
        {
            return new PerformanceMeasurer(_logger, args != null ? String.Format(name, args) : name);
        }
    }
}