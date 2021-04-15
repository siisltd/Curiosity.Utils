namespace Curiosity.Notifications
{
    /// <summary>
    /// Metadata from which a notification will be created.
    /// </summary>
    public interface INotificationMetadata
    {
        /// <summary>
        /// Type of a notification.
        /// </summary>
        string Type { get; }
    }
}