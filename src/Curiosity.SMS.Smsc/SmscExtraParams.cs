using System;

namespace Curiosity.SMS.Smsc
{
    /// <summary>
    /// Extra options for <see cref="SmscSender"/>.
    /// </summary>
    public class SmscExtraParams : ISmsExtraParams
    {
        /// <summary>
        /// Login to account at https://smsc.ru.
        /// </summary>
        public string? SmscLogin { get; }

        public string? SmscPassword { get; }
        public string? SenderName { get; }

        public SmscExtraParams(string? smscLogin, string? smscPassword, string? senderName)
        {
            var isLoginSpecified = !String.IsNullOrWhiteSpace(smscLogin);
            var isPasswordSpecified = !String.IsNullOrWhiteSpace(smscPassword);
            if (isLoginSpecified && !isPasswordSpecified)
                throw new ArgumentException("Password must be specified if login was specified", nameof(smscPassword));
            if (!isLoginSpecified && isPasswordSpecified)
                throw new ArgumentException("Login must be specified if password was specified", nameof(smscLogin));

            SmscLogin = smscLogin;
            SmscPassword = smscPassword;
            SenderName = senderName;
        }
    }
}
