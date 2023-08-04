namespace Curiosity.SMS.Iqsms;

/// <summary>
/// Extra options for <see cref="IqsmsSender"/>.
/// </summary>
public class IqsmsExtraParams : ISmsExtraParams
{
    /// <summary>
    /// Login to account at https://iqsms.ru
    /// </summary>
    public string? Login { get; }
    public string? Password { get; }
    public string? SenderName { get; }

    public IqsmsExtraParams(string? login, string? password, string? senderName)
    {
        var isLoginSpecified = !String.IsNullOrWhiteSpace(login);
        var isPasswordSpecified = !String.IsNullOrWhiteSpace(password);
        if (isLoginSpecified && !isPasswordSpecified) throw new ArgumentException("Password must be specified if login was specified", nameof(password));
        if (!isLoginSpecified && isPasswordSpecified) throw new ArgumentException("Login must be specified if password was specified", nameof(login));

        Login = login;
        Password = password;
        SenderName = senderName;
    }
}