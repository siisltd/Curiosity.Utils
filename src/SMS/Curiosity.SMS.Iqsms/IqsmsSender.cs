using System.Net;
using Curiosity.Configuration;
using Curiosity.Tools;
using Microsoft.Extensions.Logging;
using RestSharp;

namespace Curiosity.SMS.Iqsms;

/// <inheritdoc cref="IIqsmsSender"/>
public class IqsmsSender : IIqsmsSender
{
    private readonly ILogger<IqsmsSender> _logger;
    private readonly IqsmsOptions         _options;

    public IqsmsSender(ILogger<IqsmsSender> logger, IqsmsOptions options)
    {
        _logger = logger   ?? throw new ArgumentNullException(nameof(logger));
        _options = options ?? throw new ArgumentNullException(nameof(options));
        options.AssertValid();
    }

    /// <inheritdoc />
    public Task<Response<SmsSentResult>> SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
    {
        if (String.IsNullOrWhiteSpace(phoneNumber)) throw new ArgumentNullException(nameof(phoneNumber));
        if (String.IsNullOrWhiteSpace(message)) throw new ArgumentNullException(nameof(message));

        return SendSmsAsync(phoneNumber, message, _options.Login, _options.Password, _options.Sender, cancellationToken);
    }

    /// <inheritdoc />
    public Task<Response<SmsSentResult>> SendSmsAsync(string phoneNumber, string message, ISmsExtraParams extraParams, CancellationToken cancellationToken = default)
    {
        if (String.IsNullOrWhiteSpace(phoneNumber)) throw new ArgumentNullException(nameof(phoneNumber));
        if (String.IsNullOrWhiteSpace(message)) throw new ArgumentNullException(nameof(message));
        if (extraParams == null) throw new ArgumentNullException(nameof(extraParams));
        if (!(extraParams is IqsmsExtraParams iqsmsExtraParams)) throw new ArgumentException($"Only {typeof(IqsmsExtraParams)} is supported.", nameof(extraParams));

        var login = iqsmsExtraParams.Login           ?? _options.Login;
        var password = iqsmsExtraParams.Password     ?? _options.Password;
        var senderName = iqsmsExtraParams.SenderName ?? _options.Sender;

        return SendSmsAsync(phoneNumber, message, login, password, senderName, cancellationToken);
    }

    private async Task<Response<SmsSentResult>> SendSmsAsync(
        string phoneNumber,
        string message,
        string login,
        string password,
        string senderName,
        CancellationToken cancellationToken)
    {
        var client = new RestClient("https://api.iqsms.ru/messages/v2/send?text=hello_world");
        var request = new RestRequest
        {
            Method = Method.Get
        };

        request.AddQueryParameter("login", login);
        request.AddQueryParameter("password", password);
        request.AddQueryParameter("sender", senderName);
        request.AddQueryParameter("phone", phoneNumber);
        request.AddQueryParameter("text", message);

        _logger.LogInformation($"Отправляем sms на номер {phoneNumber}...");

        // execute
        var response = await client.ExecuteAsync<string>(request, cancellationToken);
        
        const string warnMessage = "Ошибка при отправке sms на номер {0} (error = \"{1}\")";
        
        // HTTP code
        switch (response.StatusCode)
        {
            case HttpStatusCode.OK:
                break;
            
            case HttpStatusCode.Unauthorized:
                _logger.LogWarning(warnMessage, phoneNumber, "Ошибка авторизации");
                return Response.Failed(
                    new Error((int)SmsError.Auth, "Ошибка авторизации"),
                    new SmsSentResult(null, null, response.Content      ?? "empty content"));
            
            default:
                _logger.LogWarning(warnMessage, phoneNumber, response.ErrorMessage);
                return Response.Failed(
                    new Error((int)SmsError.Communication, response.ErrorMessage ?? "empty error message"),
                    new SmsSentResult(null, null, response.Content               ?? "empty content"));
        }

        // unknown content
        var content = response.Content?.Split(";");
        if (content == null || content.Length < 1)
        {
            _logger.LogWarning(warnMessage, phoneNumber, "Неожиданый контент в ответе");

            return Response.Failed(
                new Error((int)SmsError.Unknown, "Неожиданый контент в ответе"),
                new SmsSentResult(null, null, response.Content ?? "empty content"));
        }

        // bad request
        var resultText = content[0];
        if (resultText != "accepted")
        {
            _logger.LogWarning(warnMessage, phoneNumber, $"Сервер вернул: {resultText}");

            return Response.Failed(
                new Error((int)SmsError.Unknown, resultText),
                new SmsSentResult(null, null, response.Content ?? "empty content"));
        }
        
        // success
        _logger.LogInformation($"Успешно отправили sms на номер {phoneNumber}. Ответ сервера: \"{response.Content}\"");

        var result = new SmsSentResult(null, null, response.Content!);
        return Response.Successful(result);
    }
}