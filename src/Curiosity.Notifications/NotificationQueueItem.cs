using System;
using System.Threading.Tasks;

namespace Curiosity.Notifications
{
    internal class NotificationQueueItem<T> where T : class, INotification
    {
        public T Notification { get; }
        public TaskCompletionSource<bool> TaskCompletionSource { get; }

        public NotificationQueueItem(T notification, TaskCompletionSource<bool> taskCompletionSource)
        {
            Notification = notification ?? throw new ArgumentNullException(nameof(notification));
            TaskCompletionSource = taskCompletionSource ?? throw new ArgumentNullException(nameof(taskCompletionSource));
        }
    }
}