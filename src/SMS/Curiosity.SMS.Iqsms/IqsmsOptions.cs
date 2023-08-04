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
    public string Sender { get; set; } = null!;

    /// <inheritdoc />
    public IReadOnlyCollection<ConfigurationValidationError> Validate(string? prefix = null)
    {
        var errors = new ConfigurationValidationErrorCollection(prefix);
        errors.AddErrorIf(String.IsNullOrEmpty(Login), nameof(Login), CanNotBeEmptyErrorDescription);
        errors.AddErrorIf(String.IsNullOrEmpty(Password), nameof(Password), CanNotBeEmptyErrorDescription);
        errors.AddErrorIf(String.IsNullOrEmpty(Sender), nameof(Sender), CanNotBeEmptyErrorDescription);

        return errors;
    }
}