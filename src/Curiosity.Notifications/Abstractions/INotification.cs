namespace Curiosity.Notifications
{
    /// <summary>
    /// Base interface for all notifications ready to sent
    /// </summary>
    public interface INotification
    {
        /// <summary>
        /// Type of the channel to send the notification to. 
        /// </summary>
        string ChannelType { get; }
    }
}