using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Curiosity.Notifications
{
    /// <inheritdoc />
    public class Notificator : INotificator
    {
        private readonly Dictionary<string, INotificationChannel> _channels;
        private readonly Dictionary<Type, Dictionary<string, INotificationBuilder>> _buildersPerType;
        private readonly ILogger _logger;

        public Notificator(
            IEnumerable<INotificationChannel> channels,
            IEnumerable<INotificationBuilder> notificationBuilders,
            ILogger logger)
        {
            if (channels == null) throw new ArgumentNullException(nameof(channels));
            if (notificationBuilders == null) throw new ArgumentNullException(nameof(notificationBuilders));

            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            // add channels
            _channels = new Dictionary<string, INotificationChannel>();
            foreach (var notificationChannel in channels)
            {
                if (_channels.ContainsKey(notificationChannel.ChannelType))
                    throw new ArgumentException($"Channel with type {notificationChannel.ChannelType} have been already registered");

                _channels[notificationChannel.ChannelType] = notificationChannel;
            }

            // add builders
            _buildersPerType = new Dictionary<Type, Dictionary<string, INotificationBuilder>>();
            foreach (var notificationBuilder in notificationBuilders)
            {
                if (!_buildersPerType.ContainsKey(notificationBuilder.NotificationType))
                    _buildersPerType[notificationBuilder.NotificationType] = new Dictionary<string, INotificationBuilder>();

                var buildersPerChannel = _buildersPerType[notificationBuilder.NotificationType];
                if (buildersPerChannel.ContainsKey(notificationBuilder.ChannelType))
                    throw new ArgumentException($"Notification builder for notification {notificationBuilder.NotificationType} " +
                                                $"for channel {notificationBuilder.ChannelType} have been already registered");

                buildersPerChannel[notificationBuilder.ChannelType] = notificationBuilder;
            }
            
            if (_channels.Count == 0) throw new ArgumentException($"You should add to IoC at least 1 implementation of {typeof(INotificationChannel)}", nameof(channels));
            if (_buildersPerType.Count == 0) throw new ArgumentException($"You should add to IoC at least 1 implementation of {typeof(INotificationBuilder)}", nameof(notificationBuilders));
        }

        /// <inheritdoc />
        public async Task NotifyAsync(INotificationMetadata notificationMetadata, CancellationToken cancellationToken = default)
        {
            if (notificationMetadata == null) throw new ArgumentNullException(nameof(notificationMetadata));

            var metadataType = notificationMetadata.GetType();
            if (!_buildersPerType.TryGetValue(metadataType, out var builders))
                throw new InvalidOperationException($"Notification builder for notification {metadataType} not found");

            // build notification for all channels
            var notificationsMap = new Dictionary<string, List<INotification>>();
            foreach (var (channelType, builder) in builders)
            {
                if (!_channels.ContainsKey(channelType))
                {
                    throw new InvalidOperationException($"No channels for type {channelType}. Notification would not be build");
                }

                var notifications = await builder.BuildNotificationsAsync(notificationMetadata, cancellationToken);
                if (notifications.Count == 0)
                {
                    _logger.LogWarning($"No notification have been built for {metadataType} for channel {channelType}");
                    continue;
                }

                if (!notificationsMap.ContainsKey(channelType))
                {
                    notificationsMap[channelType] = new List<INotification>();
                }

                for (var i = 0; i < notifications.Count; i++)
                {
                    var notification = notifications[i];
                    if (notification != null)
                    {
                        notificationsMap[channelType].Add(notification);
                    }
                }
            }

            // create sending task for each notification
            var sendingTasks = new List<Task>(notificationsMap.Count);
            foreach (var (channelType, notifications) in notificationsMap)
            {
                var channel = _channels[channelType];
                for (var i = 0; i < notifications.Count; i++)
                {
                    var notification = notifications[i];
                    sendingTasks.Add(channel.SendNotificationAsync(notification, cancellationToken));
                }
            }

            // await all
            await Task.WhenAll(sendingTasks);
        }

        /// <inheritdoc />
        public void NotifyAndForgot(INotificationMetadata notificationMetadata)
        {
            if (notificationMetadata == null) throw new ArgumentNullException(nameof(notificationMetadata));

            NotifyAsync(notificationMetadata).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    _logger.LogError(task.Exception, $"Notification task faulted (NotificationType=\"{notificationMetadata.GetType()}\")");
                }

                if (task.IsCanceled)
                {
                    _logger.LogError($"Notification task was canceled (NotificationType=\"{notificationMetadata.GetType()}\")");
                }
            });
        }
    }
}
