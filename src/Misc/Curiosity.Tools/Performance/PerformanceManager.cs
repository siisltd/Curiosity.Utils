using System;
using MemoryPools;
using Microsoft.Extensions.Logging;

namespace Curiosity.Tools.Performance
{
    /// <summary>
    /// Class for measure duration of a some code. 
    /// </summary>
    public class PerformanceManager
    {
        private static ILogger _logger = null!;

        /// <summary>
        /// Initializes manager with default values.
        /// </summary>
        /// <param name="logger">Logger in which the manager will be write logs.</param>
        public static void Initialize(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// Creates a measurer object with specified name.
        /// </summary>
        /// <param name="name">Name for measurer.</param>
        /// <returns>New instance of measurer.</returns>
        /// <remarks>
        /// New instance of measurer must be disposed.
        /// </remarks>
        public static PerformanceMeasurer Measure(string name)
        {
            if (String.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

            AsserLoggerInitialized();

            // use pulling for better performance and 0 memory traffic
            // caller must dispose measurer to return object to the pool
            var measurer = Pool<PerformanceMeasurer>.Get();
            measurer.Init(_logger, name);

            return measurer;
        }

        private static void AsserLoggerInitialized()
        {
            if (_logger == null)
                throw new InvalidOperationException($"{nameof(PerformanceManager)} is not initialized. Please, call method {nameof(Initialize)} before any usage");
        }

        /// <summary>
        /// Creates a measurer object with specified name format.
        /// </summary>
        /// <param name="name">Name format for measurer.</param>
        /// <param name="args">Arguments for format</param>
        /// <returns>New instance of measurer.</returns>
        /// <remarks>
        /// Returned object must be disposed in order to avoid memory leaks.
        /// </remarks>
        public static PerformanceMeasurer Measure(string name, params object[]? args)
        {
            if (String.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

            AsserLoggerInitialized();

            // use pulling for better performance and 0 memory traffic
            // caller must dispose measurer to return object to the pool
            var measurer = Pool<PerformanceMeasurer>.Get();
            measurer.Init(_logger, args != null ? String.Format(name, args) : name);

            return measurer;
        }
    }
}
