namespace Curiosity.Notification.Abstractions
{
    /// <summary>
    /// Base interface for all notifications ready to sent
    /// </summary>
    public interface INotification
    {
        string ChannelType { get; }
    }
}