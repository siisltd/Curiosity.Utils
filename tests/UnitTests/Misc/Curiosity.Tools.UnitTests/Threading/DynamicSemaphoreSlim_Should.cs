using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.Tools.Threading;
using FluentAssertions;
using Xunit;
using Xunit.Abstractions;

namespace Curiosity.Tools.UnitTests.Threading;

/// <summary>
/// Positive unit tests for <see cref="DynamicSemaphoreSlim"/>.
/// </summary>
public class DynamicSemaphoreSlim_Should
{
    /// <summary>
    /// Seed value for random instance
    /// </summary>
    private static int _randomSeed = Environment.TickCount;
    private static readonly ThreadLocal<Random> Random = new(() => new(Interlocked.Increment(ref _randomSeed)));

    private readonly ITestOutputHelper _testOutputHelper;

    public DynamicSemaphoreSlim_Should(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper ?? throw new ArgumentNullException(nameof(testOutputHelper));
    }


    /// <summary>
    /// Checks that semaphore works normally when we dynamically change slots.
    /// </summary>
    [Fact]
    public async Task LimitWorkTo4WithDynamicChangesThreads()
    {
        var maxConcurrencyLevel = 4;
        var minConcurrencyLevel = 1;
        var maxConcurrencyCount = 32;
        var semaphore = new DynamicSemaphoreSlim(minConcurrencyLevel, maxConcurrencyLevel, maxConcurrencyCount);

        var tasksCount = 500;

        var tasks = new List<Task>(tasksCount);
        var counter = new ConcurrencyCounter();
        for (var i = 0; i < tasksCount; i++)
        {
            // let's change concurrency level
            var randomValue = Random.Value.Next(0, 10);
            if (randomValue >= 6)
            {
                if (randomValue >= 8 && counter.Value < maxConcurrencyLevel)
                {
                    if (semaphore.TryIncrease())
                    {
                        maxConcurrencyLevel++;
                        _testOutputHelper.WriteLine(
                            "Increased semaphore level to {0}. Available slots count is {1}",
                            maxConcurrencyLevel,
                            semaphore.AvailableSlotsCount);
                    }
                    else
                    {
                        _testOutputHelper.WriteLine(
                            "Failed to increase semaphore level. Current level is {0}. Available slots count is {1}",
                            maxConcurrencyLevel,
                            semaphore.AvailableSlotsCount);
                    }
                }
                else if (counter.Value > minConcurrencyLevel)
                {
                    if (semaphore.TryDecrease())
                    {
                        maxConcurrencyLevel--;
                        _testOutputHelper.WriteLine(
                            "Decreased semaphore level to {0}. Available slots count is {1}",
                            maxConcurrencyLevel,
                            semaphore.AvailableSlotsCount);
                    }
                    else
                    {
                        _testOutputHelper.WriteLine(
                            "Failed to decrease semaphore level. Current level is {0}. Available slots count is {1}",
                            maxConcurrencyLevel,
                            semaphore.AvailableSlotsCount);
                    }
                }
            }
            
            await semaphore.WaitAsync();
            var counterValue = Interlocked.Increment(ref counter.Value);
            
            _testOutputHelper.WriteLine(
                "Current ConcurrencyLevel is {0}",
                counterValue);
            
            if (counterValue > maxConcurrencyLevel)
            {
                counterValue.Should().BeLessThanOrEqualTo(maxConcurrencyLevel, "We set concurrency level");
            }
            
            var task = DummyActionAsync();
            task.WithCompletion(_ =>
            {
                Interlocked.Decrement(ref counter.Value);
                semaphore.Release();
            });
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
    }

    private Task DummyActionAsync()
    {
        var randomDelayMs = Random.Value.Next(0, 500);
        return Task.Delay(TimeSpan.FromMilliseconds(randomDelayMs));
    }
    
    /// <summary>
    /// Checks that semaphore works normally when we don't dynamically change slots.
    /// </summary>
    [Fact]
    public async Task LimitWorkTo4WithoutDynamicChangeThreads()
    {
        const int maxConcurrencyLevel = 4;
        var semaphore = new DynamicSemaphoreSlim(1, maxConcurrencyLevel, 32);

        var tasksCount = 500;

        var tasks = new List<Task>(tasksCount);
        var counter = new ConcurrencyCounter();
        for (var i = 0; i < tasksCount; i++)
        {
            await semaphore.WaitAsync();
            var counterValue = Interlocked.Increment(ref counter.Value);
            if (counterValue > maxConcurrencyLevel)
            {
                counterValue.Should().BeLessThanOrEqualTo(maxConcurrencyLevel, "We set concurrency level");
            }
            
            var task = DummyActionAsync();
            task.WithCompletion(_ =>
            {
                Interlocked.Decrement(ref counter.Value);
                semaphore.Release();
            });
            tasks.Add(task);
        }

        await Task.WhenAll(tasks);
    }
    
    private class ConcurrencyCounter
    {
        public volatile int Value;
    }
}