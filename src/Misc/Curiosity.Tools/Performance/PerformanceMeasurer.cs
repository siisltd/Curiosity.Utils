using System;
using System.Diagnostics;
using MemoryPools;
using Microsoft.Extensions.Logging;

namespace Curiosity.Tools.Performance
{
    /// <summary>
    /// Class for measuring performance.
    /// </summary>
    public class PerformanceMeasurer : IDisposable
    {
        private ILogger? _logger;
        private readonly Stopwatch _stopwatch;
        private string _name = null!;

        /// <summary>
        /// <inheritdoc cref="PerformanceMeasurer"/>
        /// </summary>
        public PerformanceMeasurer()
        {
            _stopwatch = new Stopwatch();
        }

        internal void Init(ILogger logger, string name)
        {
            if (String.IsNullOrWhiteSpace(name)) throw new ArgumentException("Value cannot be null or whitespace.", nameof(name));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _name = name;

            _stopwatch.Start();
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_stopwatch.IsRunning)
            {
                _stopwatch.Stop();
                if (_logger != null && _logger.IsEnabled(LogLevel.Information))
                {
                    _logger.LogInformation("{MeasurerName} -> {MeasurerSpentTime} ms", _name, _stopwatch.ElapsedMilliseconds);
                }
                _stopwatch.Reset();

                Pool.Return(this);
            }
        }

        /// <summary>
        /// Replaces name of measurer for a new one.
        /// </summary>
        /// <param name="name">Name of format for name.</param>
        /// <param name="args">Arguments for name format.</param>
        public void ReplaceName(string name, params object[]? args)
        {
            _name = args != null 
                ? String.Format(name, args)
                : name;
        }
    }
}
