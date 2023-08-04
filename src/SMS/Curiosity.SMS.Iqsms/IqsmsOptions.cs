using Curiosity.Configuration;

namespace Curiosity.SMS.Iqsms;

/// <summary>
/// Options for <see cref="Curiosity.SMS.Iqsms.IqsmsSender"/>
/// </summary>
public class IqsmsOptions : IValidatableOptions, ILoggableOptions
{
    private const string CanNotBeEmptyErrorDescription = "can not be empty";

    /// <summary>
    /// Login for iqsms.
    /// </summary>
    public string Login { get; set; } = null!;

    /// <summary>
    /// Pass for iqsms
    /// </summary>
    public string Password { get; set; } = null!;

    /// <summary>
    /// Sender's name.
    /// </summary>
    public string? Sender { get; set; }

    /// <summary>
    /// When true, automatically transforms phone numbers to the form +71234567890
    /// </summary>
    /// <remarks>
    /// iqsms.ru requires phone number in format +71234567890
    /// </remarks>
    public bool AutoTransformPhoneNumber { get; set; } = true;

    /// <inheritdoc />
    public IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
    {
        var errors = new ConfigurationValidationErrorCollection(prefix);
        errors.AddErrorIf(String.IsNullOrEmpty(Login), nameof(Login), CanNotBeEmptyErrorDescription);
        errors.AddErrorIf(String.IsNullOrEmpty(Password), nameof(Password), CanNotBeEmptyErrorDescription);

        return errors;
    }
}