using System.Collections.Generic;
using Curiosity.Configuration;

namespace Curiosity.Hosting.ThreadPool
{
    /// <summary>
    /// Options for monitoring and tuning thread pool.
    /// </summary>
    public class ThreadPoolOptions: ILoggableOptions, IValidatableOptions
    {
        /// <summary>
        /// MinWorkerThreads set at startup.
        /// </summary>
        public int MinWorkerThreads { get; set; } = System.Environment.ProcessorCount * 4;

        /// <summary>
        /// MinCompletionPortThreads set at startup.
        /// </summary>
        public int MinCompletionPortThreads { get; set; } = System.Environment.ProcessorCount * 4;

        /// <summary>
        /// A period in seconds that indicates how often to log information about the thread pool state.
        /// </summary>
        public int LogThreadPoolStatePeriodSec { get; set; } = 30;

        /// <summary>
        /// Flag for enabling automatic maintenance of the minimum number of unused threads.
        /// </summary>
        public bool AutoTune { get; set; } = false;
        
        /// <summary>
        /// Expected (minimum) number of unused worker threads supported by the tuning algorithm.
        /// </summary>
        public int AutoTuneExpectedUnusedWorkerThreads { get; set; } = 8;

        /// <summary>
        /// Expected (minimum) number of unused completion port threads supported by the tuning algorithm.
        /// </summary>
        public int AutoTuneExpectedUnusedCompletionPortThreads { get; set; } = 4;

        /// <summary>
        /// The period in seconds after which the need to increase the number of unused threads will be checked.
        /// </summary>
        public int AutoTuneIncreasePeriodSec { get; set; } = 1;

        /// <summary>
        /// The period in seconds after which the need to decrease the number of unused threads will be checked.
        /// </summary>
        public int AutoTuneDecreasePeriodSec { get; set; } = 10;

        /// <summary>
        /// How many unused worker threads will be added in one step if there are not enough of them.
        /// </summary>
        public int AutoTuneIncreaseUnusedWorkerThreadsPerStep { get; set; } = 8;

        /// <summary>
        /// How many unused worker threads will be released in one step if there are too many of them.
        /// </summary>
        public int AutoTuneDecreaseUnusedWorkerThreadsPerStep { get; set; } = 1;

        /// <summary>
        /// How many unused completion port threads will be added in one step if there are not enough of them.
        /// </summary>
        public int AutoTuneIncreaseUnusedCompletionPortThreadsPerStep { get; set; } = 8;

        /// <summary>
        /// How many unused completion port threads will be released in one step if there are too many of them.
        /// </summary>
        public int AutoTuneDecreaseUnusedCompletionPortThreadsPerStep { get; set; } = 1;

        /// <inheritdoc cref="IValidatableOptions" />
        public virtual IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
        {
            var errors = new ConfigurationValidationErrorCollection(prefix);

            errors.AddErrorIf(MinWorkerThreads < 0, nameof(MinWorkerThreads), "must be greater than or equal to 0");
            errors.AddErrorIf(MinCompletionPortThreads < 0, nameof(MinCompletionPortThreads), "must be greater than or equal to 0");
            errors.AddErrorIf(LogThreadPoolStatePeriodSec < 0, nameof(LogThreadPoolStatePeriodSec), "must be greater than or equal to 0");

            if (AutoTune)
            {
                errors.AddErrorIf(AutoTuneExpectedUnusedWorkerThreads < 0, nameof(AutoTuneExpectedUnusedWorkerThreads), "must be greater than or equal to 0");
                errors.AddErrorIf(AutoTuneExpectedUnusedCompletionPortThreads < 0, nameof(AutoTuneExpectedUnusedCompletionPortThreads), "must be greater than or equal to 0");
                
                errors.AddErrorIf(AutoTuneIncreasePeriodSec <= 0, nameof(AutoTuneIncreasePeriodSec), "must be greater than 0");
                errors.AddErrorIf(AutoTuneDecreasePeriodSec <= 0, nameof(AutoTuneDecreasePeriodSec), "must be greater than 0");
                
                errors.AddErrorIf(AutoTuneIncreaseUnusedWorkerThreadsPerStep <= 0, nameof(AutoTuneIncreaseUnusedWorkerThreadsPerStep), "must be greater than 0");
                errors.AddErrorIf(AutoTuneDecreaseUnusedWorkerThreadsPerStep <= 0, nameof(AutoTuneDecreaseUnusedWorkerThreadsPerStep), "must be greater than 0");

                errors.AddErrorIf(AutoTuneIncreaseUnusedCompletionPortThreadsPerStep <= 0, nameof(AutoTuneIncreaseUnusedCompletionPortThreadsPerStep), "must be greater than 0");
                errors.AddErrorIf(AutoTuneDecreaseUnusedCompletionPortThreadsPerStep <= 0, nameof(AutoTuneDecreaseUnusedCompletionPortThreadsPerStep), "must be greater than 0");
            }

            return errors;
        }
    }
}