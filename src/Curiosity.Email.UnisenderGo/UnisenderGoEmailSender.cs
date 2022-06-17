using System;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.Configuration;
using Curiosity.EMail;
using Curiosity.Tools;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Serializers.NewtonsoftJson;

namespace Curiosity.Email.UnisenderGo
{
    /// <summary>
    /// Class for sending emails via UnisenderGo API.
    /// </summary>
    public class UnisenderGoEmailSender : IUnisenderGoEmailSender
    {
        private readonly JsonSerializerSettings _serializerSettings;
        private readonly ILogger _logger;
        private readonly UnisenderGoEmailOptions _options;

        /// <inheritdoc cref="UnisenderGoEmailSender"/>
        public UnisenderGoEmailSender(
            ILogger<UnisenderGoEmailSender> logger,
            UnisenderGoEmailOptions options)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _options = options ?? throw new ArgumentNullException(nameof(options));
            options.AssertValid();

            _serializerSettings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            };
        }

        /// <inheritdoc />
        public Task<Response> SendAsync(
            string toAddress,
            string subject,
            string body,
            bool isBodyHtml = false,
            CancellationToken cancellationToken = default)
        {
            EmailGuard.AssertToAddress(toAddress);
            EmailGuard.AssertToAddress(subject);
            EmailGuard.AssertToAddress(body);

            return SendAsync(
                toAddress,
                subject,
                body,
                isBodyHtml,
                _options.ApiKey,
                _options.EmailFrom,
                _options.FromName,
                _options.Region,
                _options.ReplyTo,
                _options.TrackLinks,
                _options.TrackReads,
                _options.UnsubscribeUrl,
                cancellationToken);
        }

        private async Task<Response> SendAsync(
            string toAddress,
            string subject,
            string body,
            bool isBodyHtml,
            string apiKey,
            string emailFrom,
            string fromName,
            UnisenderGoRegion region,
            string? replyTo,
            bool? trackLinks,
            bool? trackReads,
            string? unsubscribeUrl,
            CancellationToken cancellationToken = default)
        {
            // some basic checks
            UnisenderGoGuard.AssertApiKey(apiKey);
            UnisenderGoGuard.AssertEmailFrom(emailFrom);
            UnisenderGoGuard.AssertFromName(fromName);
            UnisenderGoGuard.AssertRegion(region);

            string unisenderGoHost;

            // select API host address based on region
            switch (region)
            {
                case UnisenderGoRegion.Russia:
                    unisenderGoHost = "https://go1.unisender.ru/ru/transactional/api/v1";
                    break;
                default:
                    throw new ArgumentException($"Region {region} is not supported.", nameof(region));
            }

            var restClient = new RestClient
            {
                BaseUrl = new Uri(unisenderGoHost)
            };
            restClient.UseNewtonsoftJson(_serializerSettings);

            // build message body
            var messageBody = new UnisenderGoSendEmailMessageBody();
            if (isBodyHtml)
            {
                messageBody.Html = body;
            }
            else
            {
                messageBody.PlainText = body;
            }

            // build message
            var message = new UnisenderGoSendEmailMessage
            {
                Recipients = new []
                {
                    new UnisenderGoRecipient
                    {
                        Email = toAddress
                    }
                },
                Body = messageBody,
                Subject = subject,
                ReplyTo = replyTo,
                FromEmail = emailFrom,
                FromName = fromName
            };

            // configure tracking
            if (trackLinks.HasValue)
            {
                message.TrackLinks = trackLinks.Value.ToUnisenderGoBool();
            }
            if (trackReads.HasValue)
            {
                message.TrackRead = trackReads.Value.ToUnisenderGoBool();
            }

            // configure extra options
            if (!String.IsNullOrWhiteSpace(unsubscribeUrl))
            {
                var options = new UnisenderGoSendEmailMessageOptions
                {
                    UnsubscribeUrl = unsubscribeUrl
                };
                message.Options = options;
            }

            var sendEmailRequest = new UnisenderGoSendEmailRequest
            {
                Message = message
            };

            // build request
            var restRequest = new RestRequest();
            restRequest.AddHeader("X-API-KEY", apiKey);
            restRequest.AddJsonBody(sendEmailRequest);
            restRequest.Resource = "email/send.json";
            restRequest.Method = Method.POST;

            // send
            _logger.LogTrace("Sending email to \"{Email}\"...", toAddress);
            var response = await restClient.ExecuteAsync(restRequest, cancellationToken);
            if (!response.IsSuccessful)
            {
                _logger.LogError(
                    "Error sending message to \"{ToAddress}\". StatusCode = {ResponseStatusCode}. Response: {ResponseContent}",
                    toAddress,
                    response.StatusCode,
                    response.Content);

                UnisenderGoSendEmailResponse? unisenderGoFailedResponse = null;
                try
                {
                    unisenderGoFailedResponse = JsonConvert.DeserializeObject<UnisenderGoSendEmailResponse>(response.Content);
                }
                catch (Exception e)
                {
                    _logger.LogWarning(e, "Error parsing UnisenderGo response");
                }

                // Unisender API is very clear, they use HTTP status code to provide more details about error
                // analyse it
                switch ((int)response.StatusCode)
                {
                    case 401:
                        return Response.Failed(new Error((int)EmailError.Auth, 
                            unisenderGoFailedResponse != null
                            ? $"UnisenderErrorCode={unisenderGoFailedResponse.Code}: {unisenderGoFailedResponse.Message}"
                            : "No API Key or API key is incorrect"));
                    case 403:
                        var emailError = EmailError.Auth;
                        string errorDescription;
                        if (unisenderGoFailedResponse != null)
                        {
                            switch (unisenderGoFailedResponse.Code)
                            {
                                case 901:
                                case 902:
                                case 905:
                                case 906:
                                    emailError = EmailError.RateLimit;
                                    break;
                                case 903:
                                    emailError = EmailError.Unknown;
                                    break;
                                default:
                                    emailError = EmailError.Auth;
                                    break;
                            }

                            errorDescription = $"UnisenderErrorCode={unisenderGoFailedResponse.Code}: {unisenderGoFailedResponse.Message}";
                        }
                        else
                        {
                            errorDescription = "Access to API is disabled / user is inactive / rate is limited";
                        }
                        return Response.Failed(new Error((int)emailError, errorDescription));
                    case 400:
                        return Response.Failed(new Error((int)EmailError.IncorrectRequestData, 
                            unisenderGoFailedResponse != null
                                ? $"UnisenderErrorCode={unisenderGoFailedResponse.Code}: {unisenderGoFailedResponse.Message}"
                                : "Something wrong with send data"));
                    case 404:
                        return Response.Failed(new Error((int)EmailError.Communication, 
                            unisenderGoFailedResponse != null
                                ? $"UnisenderErrorCode={unisenderGoFailedResponse.Code}: {unisenderGoFailedResponse.Message}"
                                : "Called method not found"));
                    case 429:
                        return Response.Failed(new Error((int)EmailError.RateLimit, 
                            unisenderGoFailedResponse != null
                                ? $"UnisenderErrorCode={unisenderGoFailedResponse.Code}: {unisenderGoFailedResponse.Message}"
                                : "To many requests"));
                    case 500:
                    case 501:
                    case 502:
                    case 503:
                        return Response.Failed(new Error((int)EmailError.Communication, 
                            unisenderGoFailedResponse != null
                                ? $"UnisenderErrorCode={unisenderGoFailedResponse.Code}: {unisenderGoFailedResponse.Message}"
                                : "UnisenderGo is unavailable. Please, try again later"));
                    default:
                        var extraInfo = $"StatusCode={(int)response.StatusCode}; Message=\"{unisenderGoFailedResponse?.Message ?? "<none>"}\"";
                        return Response.Failed(new Error((int)EmailError.Unknown, $"Unknown error. Extra info: {extraInfo}"));
                }
            }

            _logger.LogDebug(
                "Message is successfully sent to \"{Email}\". Response: {Response}",
                toAddress,
                response.Content);

            try
            {
                var unisenderGoSuccessResponse = JsonConvert.DeserializeObject<UnisenderGoSendEmailResponse>(response.Content)!;
                _logger.LogDebug(
                    "UnisenderGo response: status = \"{UnisenderGoSuccessResponseStatus}\", jobId = \"{UnisenderGoSuccessResponseJobId}\"",
                    unisenderGoSuccessResponse.Status,
                    unisenderGoSuccessResponse.JobId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error parsing UnisenderGo response for send email \"{Email}\"", toAddress);

                return Response.Failed(new Error((int)EmailError.Communication, "Incorrect UnisenderGo response"));
            }

            return Response.Successful();
        }

        /// <inheritdoc />
        public Task<Response> SendAsync(
            string toAddress,
            string subject,
            string body,
            bool isBodyHtml,
            IEMailExtraParams emailExtraParams,
            CancellationToken cancellationToken = default)
        {
            EmailGuard.AssertToAddress(toAddress);
            EmailGuard.AssertToAddress(subject);
            EmailGuard.AssertToAddress(body);

            if (emailExtraParams == null) throw new ArgumentNullException(nameof(emailExtraParams));

            UnisenderGoEmailExtraParams? unisenderGoEmailExtraParams = null;
            if (emailExtraParams is UnisenderGoEmailExtraParams @params)
            {
                unisenderGoEmailExtraParams = @params;
            }
            else
            {
                 if (!_options.IgnoreIncorrectExtraParamsType)
                     throw new ArgumentException($"Only {typeof(UnisenderGoEmailExtraParams)} is supported for this sender.", nameof(emailExtraParams));
            }

            var apiKey = unisenderGoEmailExtraParams?.ApiKey ?? _options.ApiKey;
            var emailFrom = unisenderGoEmailExtraParams?.EmailFrom ?? _options.EmailFrom;
            var fromName = unisenderGoEmailExtraParams?.FromName ?? _options.FromName;
            var region = unisenderGoEmailExtraParams?.Region ?? _options.Region;
            var replyTo = unisenderGoEmailExtraParams?.ReplyTo ?? _options.ReplyTo;
            var trackLinks = unisenderGoEmailExtraParams?.TrackLinks ?? _options.TrackLinks;
            var trackReads = unisenderGoEmailExtraParams?.TrackReads ?? _options.TrackReads;
            var unsubscribeUrl = unisenderGoEmailExtraParams?.UnsubscribeUrl ?? _options.UnsubscribeUrl;

            return SendAsync(
                toAddress,
                subject,
                body,
                isBodyHtml,
                apiKey,
                emailFrom,
                fromName,
                region,
                replyTo,
                trackLinks,
                trackReads,
                unsubscribeUrl,
                cancellationToken);
        }
    }
}
