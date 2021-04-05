using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Curiosity.Notification.Abstractions;
using Microsoft.Extensions.Logging;

namespace Curiosity.Notification
{
    public class Notificator : INotificator
    {
        private readonly Dictionary<string, INotificationChannel> _channels;
        private readonly Dictionary<string, Dictionary<string, INotificationBuilder>> _buildersPerType;
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
            _buildersPerType = new Dictionary<string, Dictionary<string, INotificationBuilder>>();
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
            
            if (_channels.Count == 0) throw new ArgumentException("Can't be empty", nameof(channels));
            if (_buildersPerType.Count == 0) throw new ArgumentException("Can't be empty", nameof(notificationBuilders));
        }

        public async Task NotifyAsync(INotificationMetadata notificationMetadata)
        {
            if (notificationMetadata == null) throw new ArgumentNullException(nameof(notificationMetadata));

            if (!_buildersPerType.TryGetValue(notificationMetadata.Type, out var builders))
                throw new InvalidOperationException($"Notification builder for notification {notificationMetadata.Type} not found");

            // build notification for all channels
            var notificationsMap = new Dictionary<string, List<INotification>>();
            foreach (var (channelType, builder) in builders)
            {
                if (!_channels.ContainsKey(channelType))
                {
                    throw new InvalidOperationException($"No channels for type {channelType}. Notification would not be build");
                }

                var notifications = await builder.BuildNotificationsAsync(notificationMetadata);
                if (notifications == null || notifications.Count == 0)
                {
                    _logger.LogWarning($"No notification have been built for {notificationMetadata.Type} for channel {channelType}");
                    continue;
                }

                if (!notificationsMap.ContainsKey(channelType))
                {
                    notificationsMap[channelType] = new List<INotification>();
                }

                foreach (var notification in notifications)
                {
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
                foreach (var notification in notifications)
                {
                    sendingTasks.Add(channel.SendNotificationAsync(notification));
                }
            }

            // await all
            await Task.WhenAll(sendingTasks);
        }

        public void NotifyAndForgot(INotificationMetadata notificationMetadata)
        {
            if (notificationMetadata == null) throw new ArgumentNullException(nameof(notificationMetadata));

            NotifyAsync(notificationMetadata).ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    _logger.LogError(task.Exception, "Notification task faulted");
                }

                if (task.IsCanceled)
                {
                    _logger.LogError("Notification task was canceled");
                }
            });
        }
    }
}