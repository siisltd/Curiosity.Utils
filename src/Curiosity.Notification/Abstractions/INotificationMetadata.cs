namespace Curiosity.Notification.Abstractions
{
    /// <summary>
    /// Metadata from which a notification will be created
    /// </summary>
    public interface INotificationMetadata
    {
        string Type { get; }
    }
}