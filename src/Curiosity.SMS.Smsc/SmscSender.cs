using System;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.Configuration;
using Curiosity.Tools;
using Curiosity.Tools.Models;
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

        public Task<Response<SmsSentResult>> SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
        {
            if (String.IsNullOrWhiteSpace(phoneNumber)) throw new ArgumentNullException(nameof(phoneNumber));
            if (String.IsNullOrWhiteSpace(message)) throw new ArgumentNullException(nameof(message));

            return SendSmsAsync(phoneNumber, message, _options.SmscLogin, _options.SmscPassword, _options.SmscSender, cancellationToken);
        }

        private async Task<Response<SmsSentResult>> SendSmsAsync(
            string phoneNumber,
            string message,
            string smscLogin,
            string smscPassword,
            string senderName,
            CancellationToken cancellationToken = default)
        {
            var client = new RestClient("https://smsc.ru/sys/send.php");
            var request = new RestRequest
            {
                Method = Method.POST
            };

            request.AddQueryParameter("login", smscLogin);
            request.AddQueryParameter("psw", smscPassword);
            request.AddQueryParameter("sender", senderName);

            request.AddQueryParameter("phones", phoneNumber);
            request.AddQueryParameter("mes", message);

            request.AddQueryParameter("cost", "2"); // отправить и вернуть стоимость
            request.AddQueryParameter("fmt", "3"); // результат в json

            _logger.LogInformation($"Отправляем sms на номер {phoneNumber}...");

            var response = await client.ExecuteAsync<SmscResponseData>(request, cancellationToken);
            bool isSuccessful;
            string resultJson;
            decimal? messageCost = null;
            if (response.IsSuccessful)
            {
                resultJson = JsonConvert.SerializeObject(response.Data, SmsConstants.JsonSerializerSettings);
                _logger.LogDebug(resultJson);
                if ((response.Data.error_code ?? 0) == 0)
                {
                    if (response.Data.cost.HasValue)
                        messageCost = response.Data.cost;

                    _logger.LogInformation($"Успешно отправили sms на номер {phoneNumber}");
                    isSuccessful = true;
                }
                else
                {
                    isSuccessful = false;
                    _logger.LogWarning($"Ошибка при отправке sms на номер {phoneNumber} (error_code = {response.Data.error_code}, error = \"{response.Data.error}\")");
                }
            }
            else
            {
                isSuccessful = false;

                var data = new SmscResponseData
                {
                    error_code = -1
                };
                if (response.ErrorException != null)
                {
                    data.error =
                        $"{response.ErrorException.GetType().Name}: {response.ErrorException.Message}";
                }
                else
                {
                    if (!String.IsNullOrEmpty(response.ErrorMessage))
                    {
                        data.error = $"{response.ErrorMessage}";
                    }
                    else
                    {
                        data.error = $"{(int) response.StatusCode}, {response.StatusDescription}";
                    }
                }

                resultJson = JsonConvert.SerializeObject(data, SmsConstants.JsonSerializerSettings);

                _logger.LogInformation(
                    $"Ошибка при отправке sms на номер {phoneNumber} (error = \"{data.error}\")");
            }

            int? sentSmsCount = null;
            if (response.IsSuccessful && response.Data != null! && response.Data.cnt > 0)
                sentSmsCount = response.Data.cnt.Value;

            var result = new SmsSentResult(sentSmsCount, messageCost, resultJson);

            return isSuccessful
                ? Response<SmsSentResult>.Successful(result)
                : Response<SmsSentResult>.Failed(new Error(-1, "Error while sending SMS"), result);
        }

        public Task<Response<SmsSentResult>> SendSmsAsync(string phoneNumber, string message, ISmsExtraParams extraParams, CancellationToken cancellationToken = default)
        {
            if (String.IsNullOrWhiteSpace(phoneNumber)) throw new ArgumentNullException(nameof(phoneNumber));
            if (String.IsNullOrWhiteSpace(message)) throw new ArgumentNullException(nameof(message));
            if (extraParams == null) throw new ArgumentNullException(nameof(extraParams));
            if (!(extraParams is SmscExtraParams smscExtraParams)) throw new ArgumentException($"Only {typeof(SmscExtraParams)} is supported.", nameof(SmscExtraParams));

            var login = smscExtraParams.SmscLogin ?? _options.SmscLogin;
            var password = smscExtraParams.SmscPassword ?? _options.SmscPassword;
            var senderName = smscExtraParams.SenderName ?? _options.SmscSender;

            return SendSmsAsync(phoneNumber, message, login, password, senderName, cancellationToken);
        }
    }
}
