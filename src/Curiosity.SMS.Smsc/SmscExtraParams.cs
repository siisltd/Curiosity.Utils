namespace Curiosity.SMS.Smsc
{
    /// <summary>
    /// Extra options for <see cref="SmscSender"/>.
    /// </summary>
    public class SmscExtraParams : ISmsExtraParams
    {
        public string? SmscLogin { get; }
        public string? SmscPassword { get; }
        public string? SenderName { get; }

        public SmscExtraParams(string? smscLogin, string? smscPassword, string? senderName)
        {
            SmscLogin = smscLogin;
            SmscPassword = smscPassword;
            SenderName = senderName;
        }
    }
}