using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Curiosity.Notifications
{
    /// <summary>
    /// Base class for all notification channels. Contains all logic for working with notifications queue.
    /// Processes one notification in time.
    /// </summary>
    /// <typeparam name="TNotification">Type of notifications to process.</typeparam>
    public abstract class NotificationChannelBase<TNotification> : BackgroundService, INotificationChannel
        where TNotification : class, INotification
    {
        /// <inheritdoc />
        public string ChannelType { get;}
        
        private readonly BlockingCollection<NotificationQueueItem<TNotification>> _notificationQueue;

        private bool _isChannelReady;
        
        /// <summary>
        /// Logger.
        /// </summary>
        protected readonly ILogger Logger;

        /// <inheritdoc cref="NotificationChannelBase{TNotification}"/>
        protected NotificationChannelBase(ILogger logger, string channelType)
        {
            ChannelType = channelType;
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _notificationQueue = new BlockingCollection<NotificationQueueItem<TNotification>>(new ConcurrentQueue<NotificationQueueItem<TNotification>>());
        }

        /// <inheritdoc />
        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation($"Starting channel \"{ChannelType}\"...");
            
            // start channel
            await base.StartAsync(cancellationToken);
            
            // mark channel as ready to receiving new notifications
            _isChannelReady = true;
            
            Logger.LogInformation($"Starting channel \"{ChannelType}\" completed.");
        }

        /// <summary>
        /// Send notification via this channel and wait for sending completion.
        /// </summary>
        /// <param name="notifications">Notification to send.</param>
        /// <param name="cancellationToken">Cancellation token.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">If args are incorrect</exception>
        /// <exception cref="InvalidOperationException">If channel is not started.</exception>
        /// <exception cref="AggregateException">If type of channel specified in notification doesn't match channel's type.</exception>
        public Task SendNotificationAsync(INotification notifications, CancellationToken cancellationToken = default)
        {
            if (notifications == null) throw new ArgumentNullException(nameof(notifications));
            if (!_isChannelReady) throw new InvalidOperationException($"Channel is not ready. Use {nameof(StartAsync)} to make channel ready");
            
            if (!(notifications is TNotification typedNotifications))
                throw new AggregateException($"\"{nameof(notifications)}\" should be type of \"{typeof(TNotification)}\"");

            var tcs = new TaskCompletionSource<bool>();

            var queueItem = new NotificationQueueItem<TNotification>(typedNotifications, tcs);
            _notificationQueue.Add(queueItem, cancellationToken);

            cancellationToken.Register(() =>
            {
                if (!tcs.TrySetCanceled())
                {
                    Logger.LogWarning($"Failed to set task for sending notification for channel \"{notifications.ChannelType}\"");
                }
            });

            return tcs.Task;
        }

        /// <inheritdoc />
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            TaskCompletionSource<bool>? currentTcs = null;
            try
            {
                while (!stoppingToken.IsCancellationRequested)
                {
                    var queueItem = _notificationQueue.Take(stoppingToken);

                    var notifications = queueItem.Notification;
                    var tcs = queueItem.TaskCompletionSource;
                    currentTcs = tcs;

                    try
                    {
                        await ProcessNotificationAsync(notifications, stoppingToken);
                        tcs.SetResult(true);
                    }
                    catch (Exception e)
                    {
                        Logger.LogWarning(e, $"Error while sending notification via channel \"{ChannelType}\". Reason: {e.Message}");
                        tcs.SetException(e);
                    }

                    currentTcs = null;
                }
            }
            // Stopping, alright
            catch (Exception) when (stoppingToken.IsCancellationRequested)
            {
                _isChannelReady = false;
                Logger.LogInformation($"Processing notification by channel \"{ChannelType}\" was canceled");
                currentTcs?.TrySetCanceled();
                ProcessQueueOnChannelStop(true);
            }
            catch (Exception ex)
            {
                _isChannelReady = false;
                Logger.LogError(ex, $"Error while processing notifications by channel \"{ChannelType}\". Reason: {ex.Message}");
                currentTcs?.TrySetException(ex);
                ProcessQueueOnChannelStop(false, ex);
            }
        }

        /// <summary>
        /// Processed specified notification.
        /// </summary>
        protected abstract Task ProcessNotificationAsync(TNotification notification, CancellationToken cancellationToken = default);

        /// <summary>
        /// Processes queued notification on channel stop.
        /// </summary>
        /// <param name="isCanceled">Was channel work stopped.</param>
        /// <param name="ex">Exception that caused the stopping.</param>
        private void ProcessQueueOnChannelStop(bool isCanceled, Exception? ex = null)
        {
            if (_notificationQueue.Count <= 0) return;

            while (_notificationQueue.TryTake(out var queueItem, TimeSpan.FromMilliseconds(10)))
            {
                string message;
                if (isCanceled || ex == null)
                {
                    queueItem.TaskCompletionSource.SetCanceled();
                    message = "Notification was canceled";
                }
                else
                {
                    queueItem.TaskCompletionSource.SetException(ex);
                    message = "Notification terminated with error";
                }

                Logger.LogWarning(message);
            }
        }

        /// <inheritdoc />
        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation($"Stopping channel \"{ChannelType}\"...");

            // mark channel as not ready to stop add new notifications to queue
            _isChannelReady = false;
            
            // stop processing current queue
            await base.StopAsync(cancellationToken);

            // process not sent notification in the queue
            ProcessQueueOnChannelStop(true);
            
            Logger.LogInformation($"Stopping channel \"{ChannelType}\" completed.");
        }
    }
}
