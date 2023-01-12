using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Curiosity.Tools
{
    /// <summary>
    /// Methods for safely launching actions in the background
    /// </summary>
    public static class FireAndForget
    {
        private static ILogger? _logger;

        /// <summary>
        /// Sets a logger that will be used for logging errors that occurred when calling the error handler
        /// </summary>
        public static void SetLogger(ILogger? logger)
        {
            _logger = logger;
        }
        
        /// <summary>
        /// Runs specified action in a background.
        /// </summary>
        /// <param name="action">Action to run.</param>
        /// <param name="exceptionHandler">Exception handler</param>
        /// <param name="isLongRunning">Is action long running.</param>
        public static void Run(Action action, Action<Exception> exceptionHandler, bool isLongRunning = false)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (exceptionHandler == null) throw new ArgumentNullException(nameof(exceptionHandler));

            var task = isLongRunning ? new Task(action, TaskCreationOptions.LongRunning) : new Task(action);

            task.ContinueWith(
                t =>
                {
                    // faulted - unhandled exception, cancelled - unhandled OperationCancelledException
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        try
                        {
                            exceptionHandler.Invoke(t.Exception);
                        }
                        catch (Exception e)
                        {
                            _logger?.LogError(e, $"Failed to correctly handle an Action error in FireAndForget. Reason: {e.Message}");
                        }
                    }
                },
                TaskContinuationOptions.ExecuteSynchronously);
            task.ConfigureAwait(false);
            task.Start();
        }

        public static void Run(Action action, ILogger errorLogger, bool isLongRunning = false)
        {
            if (action == null) throw new ArgumentNullException(nameof(action));
            if (errorLogger == null) throw new ArgumentNullException(nameof(errorLogger));
            
            Run(
                action,
                e =>
                {
                    errorLogger.LogError(e, $"Action crashed with an error in FireAndForgot: {e.Message}");
                },
                isLongRunning);
        }
        
        /// <summary>
        /// Attaches an error handler to the task
        /// </summary>
        /// <param name="task">Task</param>
        /// <param name="exceptionHandler">Handler that will be called in case of an error</param>
        public static void WithExceptionHandler(this Task task, Action<Exception> exceptionHandler)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            if (exceptionHandler == null) throw new ArgumentNullException(nameof(exceptionHandler));

            task.ConfigureAwait(false);
            task.ContinueWith(
                t =>
                {
                    // faulted - unhandled exception, cancelled - unhandled OperationCancelledException
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        try
                        {
                            exceptionHandler.Invoke(t.Exception);
                        }
                        catch (Exception e)
                        {
                            _logger?.LogError(e, $"Failed to correctly handle the Task' error in FireAndForget. Reason: {e.Message}");
                        }
                    }
                },
                TaskContinuationOptions.ExecuteSynchronously);
        }     
        
        /// <summary>
        /// Attaches an error handler to the task
        /// </summary>
        /// <param name="task">Task</param>
        /// <param name="errorHandler">Handler that will be called in case of an error</param>
        public static void WithExceptionHandler(this Task task, Func<Exception, Task> errorHandler)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            if (errorHandler == null) throw new ArgumentNullException(nameof(errorHandler));

            task.ConfigureAwait(false);
            task.ContinueWith(
                async t =>
                {
                    // faulted - unhandled exception, cancelled - unhandled OperationCancelledException
                    if (t.IsFaulted || t.IsCanceled)
                    {
                        try
                        {
                            await errorHandler.Invoke(t.Exception);
                        }
                        catch (Exception e)
                        {
                            _logger?.LogError(e, $"Не удалось корректно обработать ошибку Task' в FireAndForget. Причина: {e.Message}");
                        }
                    }
                },
                TaskContinuationOptions.ExecuteSynchronously);
        }

        /// <summary>
        /// Attaches a handler with a logger to the task for error handling
        /// </summary>
        /// <param name="task">Task</param>
        /// <param name="errorLogger">The logger to which the error will be recorded if it occurs</param>
        public static void WithExceptionLogger(this Task task, ILogger errorLogger)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            if (errorLogger == null) throw new ArgumentNullException(nameof(errorLogger));

            WithExceptionHandler(
                task,
                e =>
                {
                    errorLogger.LogError(e, $"Task crashed with an error after FireAndForget:{e.Message}");
                });
        }

        /// <summary>
        /// Correctly and safely adds to task action that will be executed on task completion.
        /// </summary>
        public static void WithCompletion(this Task task, Action<Task> action)
        {
            if (task == null) throw new ArgumentNullException(nameof(task));
            if (action == null) throw new ArgumentNullException(nameof(action));

            task.ConfigureAwait(false);
            task.ContinueWith(t =>
            {
                try
                {
                    action.Invoke(t);
                }
                catch (Exception e)
                {
                    _logger?.LogError(e, $"Не удалось корректно обработать завершение Task'и в FireAndForget. Причина: {e.Message}");
                }
            }, TaskContinuationOptions.ExecuteSynchronously);
        }
    }
}
