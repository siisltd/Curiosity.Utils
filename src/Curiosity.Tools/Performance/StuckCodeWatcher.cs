using System;
using System.Collections.Concurrent;
using System.Globalization;
using MemoryPools;
using Microsoft.Extensions.Logging;

namespace Curiosity.Tools.Performance
{
    /// <summary>
    /// Stuck code detector. Allows you to set the time for a piece of code to be executed.
    /// If block of code doesn't completed for a specified time, the information about it will appear in the log.
    /// </summary>
    internal class StuckCodeWatcher
    {
        private struct StuckCodeItem
        {
            public int Key;
            public string Label;
            public DateTime Entered;
            public int Timeout;
            public int ThreadId;
        }

        private readonly ConcurrentDictionary<int, StuckCodeItem> _items = new ConcurrentDictionary<int, StuckCodeItem>();

        private class StuckCodeItemFinalizer : IDisposable
        {
            private int _key;
            private StuckCodeWatcher _watcher = null!;

            public void Init(StuckCodeWatcher watcher, int key)
            {
                _key = key;
                _watcher = watcher;
            }

            public void Dispose()
            {
                Pool<StuckCodeItemFinalizer>.Return(this);
                _watcher.Leave(_key);
            }
        }

        public IDisposable Enter(string label, int timeoutSeconds)
        {
            if (String.IsNullOrEmpty(label))
                throw new ArgumentNullException(nameof(label));
            if (timeoutSeconds < 1)
                throw new ArgumentOutOfRangeException(nameof(timeoutSeconds));

            var now = DateTime.Now;
            var item = new StuckCodeItem
                {
                    Entered = now,
                    Label = label,
                    Timeout = timeoutSeconds,
                    Key = $"{label} #{now.Ticks}".GetHashCode(),
                    ThreadId = System.Threading.Thread.CurrentThread.ManagedThreadId
                };

            _items.TryAdd(item.Key, item);

            var finalizer = Pool<StuckCodeItemFinalizer>.Get();
            finalizer.Init(this, item.Key);

            return finalizer;
        }

        private void Leave(int key)
        {
            _items.TryRemove(key, out _);
        }
        
        private static readonly CultureInfo LoggerCi = CultureInfo.GetCultureInfo("ru-RU");

        public void Validate(ILogger? logger)
        {
            var now = DateTime.Now;

            var itemsSnapshot = _items.Values;
            
            foreach (var item in itemsSnapshot)
            {
                int duration = Convert.ToInt32(now.Subtract(item.Entered).TotalSeconds);
                if (duration >= item.Timeout)
                {
                    logger?.LogError("STUCK CODE: \"{StuckCodeLabel}\", duration {StuckCodeDuration} (entered at {StuckCodeEnterTime} from thread #{StuckCodeThreadId})",
                        item.Label,
                        duration.ToMinSec(),
                        item.Entered.ToString(LoggerCi),
                        item.ThreadId);
                }
            }
        }
    }
}
