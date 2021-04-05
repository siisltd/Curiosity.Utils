using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.Notification.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Curiosity.Notification.Channels
{
    public abstract class NotificationChannelBase<TNotification> : BackgroundService, INotificationChannel
        where TNotification : class, INotification
    {
        public string ChannelType { get; }
        
        private readonly BlockingCollection<NotificationQueueItem<TNotification>> _notificationQueue;
        protected readonly ILogger Logger;
        private bool _isChannelReady;

        protected NotificationChannelBase(ILogger logger, string channelType)
        {
            ChannelType = channelType;
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _notificationQueue = new BlockingCollection<NotificationQueueItem<TNotification>>(new ConcurrentQueue<NotificationQueueItem<TNotification>>());
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            Logger.LogInformation($"Starting chanel with type: \"{ChannelType}\"...");
            await base.StartAsync(cancellationToken);
            _isChannelReady = true;
            Logger.LogInformation($"Starting chanel with type: \"{ChannelType}\" completed.");
        }

        public Task SendNotificationAsync(INotification notifications)
        {
            if (notifications == null) throw new ArgumentNullException(nameof(notifications));
            if (!_isChannelReady) throw new InvalidOperationException($"Channel is not ready. Use {nameof(StartAsync)} to make channel ready");
            
            if (!(notifications is TNotification typedNotifications))
                throw new AggregateException($"{nameof(notifications)} should be type of {typeof(TNotification)}");

            var tcs = new TaskCompletionSource<bool>();

            var queueItem = new NotificationQueueItem<TNotification>(typedNotifications, tcs);
            _notificationQueue.Add(queueItem);

            return tcs.Task;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await Task.Yield();

            TaskCompletionSource<bool> currentTcs = null;
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
                        await ProcessNotificationAsync(notifications);
                        tcs.SetResult(true);
                    }
                    catch (Exception e)
                    {
                        Logger.LogError(e, $"Error while sending notification by channel with type: \"{ChannelType}\". Reason: {e.Message}");
                        tcs.SetException(e);
                    }

                    currentTcs = null;
                }
            }
            // Stopping, alright
            catch (Exception) when (stoppingToken.IsCancellationRequested)
            {
                _isChannelReady = false;
                Logger.LogInformation($"Processing notification by channel with type: \"{ChannelType}\" was canceled");
                currentTcs?.SetCanceled();
                ProcessQueueOnTermination(isCanceled: true);
            }
            catch (Exception ex)
            {
                _isChannelReady = false;
                Logger.LogError(ex, $"Error while processing notifications by channel with type: \"{ChannelType}\". Reason: {ex.Message}");
                currentTcs?.SetException(ex);
                ProcessQueueOnTermination(isCanceled: false, ex);
            }
        }

        protected abstract Task ProcessNotificationAsync(TNotification notification);

        private void ProcessQueueOnTermination(bool isCanceled, Exception ex = null)
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

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _isChannelReady = false;
            ProcessQueueOnTermination(isCanceled: true);
            Logger.LogInformation($"Stopping channel with type: \"{ChannelType}\"...");
            await base.StopAsync(cancellationToken);
            Logger.LogInformation($"Stopping channel with type: \"{ChannelType}\" completed.");
        }
    }
}