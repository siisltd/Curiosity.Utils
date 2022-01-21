using System;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Curiosity.Tools.Threading
{
    /// <summary>
    /// An improvement over System.Threading.SemaphoreSlim that allows you to dynamically increase and
    /// decrease the number of threads that can access a resource or pool of resources concurrently.
    /// </summary>
    /// <seealso cref="System.Threading.SemaphoreSlim" />
    public class DynamicSemaphoreSlim : SemaphoreSlim
    {
        private readonly ILogger? _logger;
        private readonly object _lockObject;

        /// <summary>
        /// Gets the minimum number of slots.
        /// </summary>
        /// <value>
        /// The minimum slots count.
        /// </value>
        public int MinimumSlotsCount { get; }

        /// <summary>
        /// Gets the number of slots currently available.
        /// </summary>
        /// <value>
        /// The available slots count.
        /// </value>
        public int AvailableSlotsCount { get; private set; }

        /// <summary>
        /// Gets the maximum number of slots.
        /// </summary>
        /// <value>
        /// The maximum slots count.
        /// </value>
        public int MaximumSlotsCount { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DynamicSemaphoreSlim"/> class.
        /// </summary>
        /// <param name="minCount">The minimum number of slots.</param>
        /// <param name="initialCount">The initial number of slots.</param>
        /// <param name="maxCount">The maximum number of slots.</param>
        /// <param name="logger">Logger</param>
        public DynamicSemaphoreSlim(
            int minCount,
            int initialCount,
            int maxCount,
            ILogger? logger = null)
            : base(initialCount, maxCount)
        {
            _logger = logger;
            _lockObject = new object();

            MinimumSlotsCount = minCount;
            AvailableSlotsCount = initialCount;
            MaximumSlotsCount = maxCount;
        }

        /// <summary>
        /// Attempts to increase the number of slots
        /// </summary>
        /// <param name="millisecondsTimeout">The timeout in milliseconds.</param>
        /// <param name="increaseCount">The number of slots to add.</param>
        /// <returns>true if the attempt was successfully; otherwise, false.</returns>
        public bool TryIncrease(int millisecondsTimeout = 500, int increaseCount = 1)
        {
            return TryIncrease(TimeSpan.FromMilliseconds(millisecondsTimeout), increaseCount);
        }

        /// <summary>
        /// Attempts to increase the number of slots
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <param name="increaseCount">The number of slots to add.</param>
        /// <returns>true if the attempt was successfully; otherwise, false.</returns>
        public bool TryIncrease(TimeSpan timeout, int increaseCount = 1)
        {
            if (increaseCount < 0) throw new ArgumentOutOfRangeException(nameof(increaseCount));
            if (increaseCount == 0) return false;

            var increased = false;

            if (AvailableSlotsCount >= MaximumSlotsCount) return increased;

            if (Monitor.TryEnter(_lockObject, timeout))
            {
                try
                {
                    for (var i = 0; i < increaseCount; i++)
                    {
                        if (AvailableSlotsCount < MaximumSlotsCount)
                        {

                            Release();
                            AvailableSlotsCount++;
                            increased = true;
                        }
                    }

                    if (increased) _logger?.LogTrace($"Semaphore slots increased: {AvailableSlotsCount}");
                }
                catch (SemaphoreFullException e)
                {
                    // An exception is thrown if we attempt to exceed the max number of concurrent tasks
                    // It's safe to ignore this exception
                    _logger?.LogWarning(e, "Failed to increase dynamic semaphore slots by {IncreaseCount}", increaseCount);
                }
                finally
                {
                    Monitor.Exit(_lockObject);
                }
            }
            else
            {
                _logger?.LogWarning(
                    "Failed to decrease dynamic semaphore slots by {IncreaseCount} because we can't enter monitor (CurrentSlots={CurrentSlots}, AvailableSlots={AvailableSlots})",
                    increaseCount,
                    CurrentCount,
                    AvailableSlotsCount);
            }

            return increased;
        }

        /// <summary>
        /// Attempts to decrease the number of slots
        /// </summary>
        /// <param name="millisecondsTimeout">The timeout in milliseconds.</param>
        /// <param name="decreaseCount">The number of slots to remove.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>true if the attempt was successfully; otherwise, false.</returns>
        public bool TryDecrease(int millisecondsTimeout = 500, int decreaseCount = 1, CancellationToken cancellationToken = default)
        {
            return TryDecrease(TimeSpan.FromMilliseconds(millisecondsTimeout), decreaseCount, cancellationToken);
        }

        /// <summary>
        /// Attempts to decrease the number of slots
        /// </summary>
        /// <param name="timeout">The timeout.</param>
        /// <param name="decreaseCount">The number of slots to remove.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns>true if the attempt was successfully; otherwise, false.</returns>
        public bool TryDecrease(TimeSpan timeout, int decreaseCount = 1, CancellationToken cancellationToken = default)
        {
            if (decreaseCount < 0) throw new ArgumentOutOfRangeException(nameof(decreaseCount));
            if (decreaseCount == 0) return false;

            var decreased = false;
            if (AvailableSlotsCount <= MinimumSlotsCount) return decreased;

            if (Monitor.TryEnter(_lockObject, timeout))
            {
                try
                {
                    var actualDecreaseValue = 0;
                    for (int i = 0; i < decreaseCount; i++)
                    {
                        if (AvailableSlotsCount <= MinimumSlotsCount) continue;

                        if (Wait(timeout, cancellationToken))
                        {
                            AvailableSlotsCount--;
                            actualDecreaseValue++;
                            decreased = true;
                        }
                    }

                    if (decreased)
                        _logger?.LogTrace(
                            "Semaphore slots decreased. Requested decrease by {ExpectedDecreaseCount}, actual decrease count is {ActualDecreaseCount} (CurrentSlots={CurrentSlots}, AvailableSlots={AvailableSlots})",
                            decreaseCount,
                            actualDecreaseValue,
                            CurrentCount,
                            AvailableSlotsCount);
                }
                catch (SemaphoreFullException e)
                {
                    // An exception is thrown if we attempt to exceed the max number of concurrent tasks
                    // It's safe to ignore this exception
                    _logger?.LogWarning(
                        e,
                        "Failed to decrease dynamic semaphore slots by {DecreaseCount} because of SemaphoreFullException (CurrentSlots={CurrentSlots}, AvailableSlots={AvailableSlots})",
                        decreaseCount,
                        CurrentCount,
                        AvailableSlotsCount);
                }
                finally
                {
                    Monitor.Exit(_lockObject);
                }
            }
            else
            {
                _logger?.LogDebug(
                    "Failed to decrease dynamic semaphore slots by {DecreaseCount} because we can't enter monitor (CurrentSlots={CurrentSlots}, AvailableSlots={AvailableSlots})",
                    decreaseCount,
                    CurrentCount,
                    AvailableSlotsCount);
            }

            return decreased;
        }
    }
}
