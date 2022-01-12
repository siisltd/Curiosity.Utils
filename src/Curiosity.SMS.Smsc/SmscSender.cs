using System;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.Configuration;
using Curiosity.Tools;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using SIISLtd.SSNG.ISACR.Core;

namespace Curiosity.SMS.Smsc
{
    /// <inheritdoc />
    public class SmscSender : ISmscSender
    {
        private readonly ILogger _logger;
        private readonly SmscOptions _options;

        public SmscSender(ILogger<SmscSender> logger, SmscOptions options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            options.AssertValid();
        }

        /// <inheritdoc />
        public Task<Response<SmsSentResult>> SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
        {
            if (String.IsNullOrWhiteSpace(phoneNumber)) throw new ArgumentNullException(nameof(phoneNumber));
            if (String.IsNullOrWhiteSpace(message)) throw new ArgumentNullException(nameof(message));

            return SendSmsAsync(phoneNumber, message, _options.SmscLogin, _options.SmscPassword, _options.SmscSender, 0, cancellationToken);
        }

        private async Task<Response<SmsSentResult>> SendSmsAsync(
            string phoneNumber,
            string message,
            string smscLogin,
            string smscPassword,
            string? senderName,
            int retriesCount = 0,
            CancellationToken cancellationToken = default)
        {
            var client = new RestClient("https://smsc.ru/sys/send.php");
            var request = new RestRequest
            {
                Method = Method.POST
            };

            request.AddQueryParameter("login", smscLogin);
            request.AddQueryParameter("psw", smscPassword);

            if (!String.IsNullOrWhiteSpace(senderName))
            {
                request.AddQueryParameter("sender", senderName);
            }

            request.AddQueryParameter("phones", phoneNumber);
            request.AddQueryParameter("mes", message);

            request.AddQueryParameter("cost", "2"); // отправить и вернуть стоимость
            request.AddQueryParameter("fmt", "3"); // результат в json

            _logger.LogInformation($"Отправляем sms на номер {phoneNumber}...");

            var response = await client.ExecuteAsync<SmscResponseData>(request, cancellationToken);
            string resultJson;
            decimal? messageCost = null;
            if (response.IsSuccessful)
            {
                resultJson = JsonConvert.SerializeObject(response.Data, SmsConstants.JsonSerializerSettings);
                _logger.LogDebug(resultJson);
                if ((response.Data.ErrorCode ?? 0) == 0)
                {
                    if (response.Data.Cost.HasValue)
                        messageCost = response.Data.Cost;

                    _logger.LogInformation($"Успешно отправили sms на номер {phoneNumber}");
                }
                else
                {
                    // if we got message denied error (6) and it is our first attempt, let's remove sender name and try again
                    // because SMSC can block SMS to Megafon and Tele2 with specified sender name when there is sender name is not agreed
                    if (response.Data.ErrorCode == 6 && retriesCount == 0)
                    {
                        _logger.LogWarning("Отправка SMS на номер {PhoneNumber} заблокирована. Попробуем повторно отправить без указания имени отправтиеля (текущее имя отправителя = \"{SenderName}\")", phoneNumber, senderName);
                        return await SendSmsAsync(phoneNumber, message, smscLogin, smscPassword, null, 1, cancellationToken);
                    }

                    var errorMessage = $"Ошибка при отправке sms на номер {phoneNumber} (error_code = {response.Data.ErrorCode}, error = \"{response.Data.Error}\")";
                    _logger.LogWarning($"Ошибка при отправке sms на номер {phoneNumber} (error_code = {response.Data.ErrorCode}, error = \"{response.Data.Error}\")");
                    return Response<SmsSentResult>.Failed(new Error((int) SmsError.Unknown, errorMessage), new SmsSentResult(null, null, resultJson));
                }
            }
            else
            {
                var data = new SmscResponseData
                {
                    ErrorCode = -1
                };
                if (response.ErrorException != null)
                {
                    data.Error =
                        $"{response.ErrorException.GetType().Name}: {response.ErrorException.Message}";
                }
                else
                {
                    data.Error = !String.IsNullOrEmpty(response.ErrorMessage)
                        ? $"{response.ErrorMessage}"
                        : $"{(int) response.StatusCode}, {response.StatusDescription}";
                }

                resultJson = JsonConvert.SerializeObject(data, SmsConstants.JsonSerializerSettings);

                _logger.LogWarning(
                    $"Ошибка при отправке sms на номер {phoneNumber} (error = \"{data.Error}\")");

                return Response<SmsSentResult>.Failed(new Error((int)SmsError.Unknown, data.Error), new SmsSentResult(null, null, resultJson));
            }

            int? sentSmsCount = null;
            if (response.IsSuccessful && response.Data != null! && response.Data.Count > 0)
                sentSmsCount = response.Data.Count.Value;

            var result = new SmsSentResult(sentSmsCount, messageCost, resultJson);

            return Response<SmsSentResult>.Successful(result);
        }

        /// <inheritdoc />
        public Task<Response<SmsSentResult>> SendSmsAsync(string phoneNumber, string message, ISmsExtraParams extraParams, CancellationToken cancellationToken = default)
        {
            if (String.IsNullOrWhiteSpace(phoneNumber)) throw new ArgumentNullException(nameof(phoneNumber));
            if (String.IsNullOrWhiteSpace(message)) throw new ArgumentNullException(nameof(message));
            if (extraParams == null) throw new ArgumentNullException(nameof(extraParams));
            if (!(extraParams is SmscExtraParams smscExtraParams)) throw new ArgumentException($"Only {typeof(SmscExtraParams)} is supported.", nameof(SmscExtraParams));

            var login = smscExtraParams.SmscLogin ?? _options.SmscLogin;
            var password = smscExtraParams.SmscPassword ?? _options.SmscPassword;
            var senderName = smscExtraParams.SenderName ?? _options.SmscSender;

            return SendSmsAsync(phoneNumber, message, login, password, senderName, 0, cancellationToken);
        }
    }
}
