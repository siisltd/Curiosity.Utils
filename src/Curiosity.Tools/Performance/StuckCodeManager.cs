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
        private static ILogger? _logger;

        private static readonly StuckCodeWatcher Watcher = new StuckCodeWatcher();
        private static readonly Timer Timer = new Timer(ValidateCallback);

        private static bool _shutdownCalled = false;

        public static void Initialize(ILogger? log = null)
        {
            if (log != null)
            {
                _logger = log;
            }

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
                _logger?.LogError(e, "StuckCodeManager.ValidateCallback() failed");
            }

            if (!_shutdownCalled)
            {
                StartTimer();
            }
        }

        public static IDisposable Enter(string label, int timeoutSeconds)
        {
            return Watcher.Enter(label, timeoutSeconds);
        }
    }
}