using System;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Curiosity.Tools.Performance
{
    /// <summary>
    /// Manager for detecting "stuck" code.
    /// </summary>
    public static class StuckCodeManager
    {
        private static ILogger _logger = null!;

        private static readonly StuckCodeWatcher Watcher = new StuckCodeWatcher();
        private static readonly Timer Timer = new Timer(ValidateCallback);

        private static bool _shutdownCalled;

        /// <summary>
        /// Initialized manager.
        /// </summary>
        /// <param name="logger">Logger in which the manager will be write logs.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void Initialize(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            StartTimer();
        }

        private static void StartTimer()
        {
            Timer.Change(30 * 1000, 0);
        }

        public static void Shutdown()
        {
            Timer.Change(Timeout.Infinite, Timeout.Infinite);
            _shutdownCalled = true;
        }

        private static void ValidateCallback(object state)
        {
            try
            {
                Watcher.Validate(_logger);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "StuckCodeManager.ValidateCallback() failed");
            }

            if (!_shutdownCalled)
            {
                StartTimer();
            }
        }

        /// <summary>
        /// Start monitoring for stuck code. If code isn't executed in specified in <see cref="timeoutSeconds"/> time, manager will write an entry to the log.
        /// </summary>
        /// <param name="label">Label for code fragment.</param>
        /// <param name="timeoutSeconds">Timeout for code execution.</param>
        /// <returns>Stuck code monitor instance.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <remarks>
        /// Returned object must be disposed in order to avoid memory leaks.
        /// </remarks>
        public static IDisposable Enter(string label, int timeoutSeconds)
        {
            if (_logger == null)
                throw new InvalidOperationException($"{nameof(StuckCodeManager)} is not initialized. Please, call method {nameof(Initialize)} before any usage");

            return Watcher.Enter(label, timeoutSeconds);
        }
    }
}
