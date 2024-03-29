using System;
using System.Threading;
using System.Threading.Tasks;
using Curiosity.Configuration;
using Curiosity.Tools;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using RestSharp;
using RestSharp.Authenticators;

namespace Curiosity.EMail.Mailgun
{
    /// <inheritdoc />
    public class MailgunEmailSender : IMailgunEmailSender
    {
        private readonly ILogger<MailgunEmailSender> _logger;
        private readonly MailgunEmailOptions _mailgunEmailOptions;

        /// <inheritdoc cref="MailgunEmailSender"/>
        public MailgunEmailSender(
            ILogger<MailgunEmailSender> logger,
            MailgunEmailOptions mailgunEmailOptions)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _mailgunEmailOptions = mailgunEmailOptions ?? throw new ArgumentNullException(nameof(mailgunEmailOptions));
            _mailgunEmailOptions.AssertValid();
        }

        private class MailGunResponse
        {
            [JsonProperty("message")]
            public string Message { get; set; } = null!;

            [JsonProperty("id")]
            public string Id { get; set; } = null!;
        }

        /// <inheritdoc />
        public Task<Response> SendAsync(string toAddress, string subject, string body, bool isBodyHtml = false, CancellationToken cancellationToken = default)
        {
            EmailGuard.AssertToAddress(toAddress);
            EmailGuard.AssertSubject(subject);
            EmailGuard.AssertBody(body);

            return SendAsync(
                toAddress,
                subject,
                body,
                isBodyHtml,
                _mailgunEmailOptions.MailgunUser,
                _mailgunEmailOptions.MailgunApiKey,
                _mailgunEmailOptions.MailgunDomain,
                _mailgunEmailOptions.EmailFrom,
                _mailgunEmailOptions.MailgunRegion,
                _mailgunEmailOptions.ReplyTo,
                cancellationToken);
        }

        private async Task<Response> SendAsync(
            string toAddress,
            string subject,
            string body,
            bool isBodyHtml,
            string mailgunUser,
            string mailGunApiKey,
            string mailgunDomain,
            string emailFrom,
            MailgunRegion region,
            string? replyTo,
            CancellationToken cancellationToken = default)
        {
            string mailgunHost;

            switch (region)
            {
                case MailgunRegion.US:
                    mailgunHost = "https://api.mailgun.net/v3";
                    break;
                case MailgunRegion.EU:
                    mailgunHost = "https://api.eu.mailgun.net/v3";
                    break;
                default:
                    throw new ArgumentException($"Region {region} is not supported.", nameof(region));
            }

            var restClient = new RestClient(new Uri(mailgunHost))
            {
                Authenticator = new HttpBasicAuthenticator(mailgunUser, mailGunApiKey)
            };

            var restRequest = new RestRequest();
            restRequest.AddParameter("domain", mailgunDomain, ParameterType.UrlSegment);
            restRequest.Resource = "{domain}/messages";
            restRequest.AddParameter("from", emailFrom);
            restRequest.AddParameter("to", toAddress);

            // add reply to address if it specified
            if (!String.IsNullOrWhiteSpace(replyTo))
                restRequest.AddParameter("h:Reply-T", replyTo);

            restRequest.AddParameter("subject", subject);
            restRequest.AddParameter(isBodyHtml ? "html" : "text", body);
            restRequest.Method = Method.Post;

            _logger.LogTrace("Sending email to {Email}...", toAddress);
            var response = await restClient.ExecuteAsync(restRequest, cancellationToken);
            if (!response.IsSuccessful)
            {
                _logger.LogWarning($"Error sending message to {toAddress}. StatusCode = {response.StatusCode.ToString()}. Response: {response.Content}");

                return ((int)response.StatusCode) == 420
                ? Response.Failed(new Error((int)EmailError.RateLimit, response.Content))
                : Response.Failed(new Error((int)EmailError.Auth, response.Content));
            }

            _logger.LogDebug("Message is successfully sent to {Email}. Response: {Response}", toAddress, response.Content);
            if (response.ContentType != "application/json") return Response.Successful();

            try
            {
                var mgResponse = JsonConvert.DeserializeObject<MailGunResponse>(response.Content);
                _logger.LogDebug($"MailGun response: message = \"{mgResponse.Message}\", id = \"{mgResponse.Id}\"");
            }
            catch (Exception e)
            {
                _logger.LogError(e, "Error parsing mailgun response");

                return Response.Failed(new Error((int)EmailError.Communication, "Incorrect mailgun response"));
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
            EmailGuard.AssertSubject(subject);
            EmailGuard.AssertBody(body);

            if (emailExtraParams == null) throw new ArgumentNullException(nameof(emailExtraParams));

            MailgunEmailExtraParams? mailGunEMailExtraParams = null;
            if (emailExtraParams is MailgunEmailExtraParams @params)
            {
                mailGunEMailExtraParams = @params;
            }
            else
            {
                if (!_mailgunEmailOptions.IgnoreIncorrectExtraParamsType)
                    throw new ArgumentException($"Only {typeof(MailgunEmailExtraParams)} is supported for this sender.", nameof(emailExtraParams));
            }

            var user = mailGunEMailExtraParams?.MailgunUser ?? _mailgunEmailOptions.MailgunUser;
            var apiKey = mailGunEMailExtraParams?.MailgunApiKey ?? _mailgunEmailOptions.MailgunApiKey;
            var domain = mailGunEMailExtraParams?.MailgunDomain ?? _mailgunEmailOptions.MailgunDomain;
            var emailFrom = mailGunEMailExtraParams?.EmailFrom ?? _mailgunEmailOptions.EmailFrom;
            var region = mailGunEMailExtraParams?.MailgunRegion ?? _mailgunEmailOptions.MailgunRegion;

            return SendAsync(
                toAddress,
                subject,
                body,
                isBodyHtml,
                user,
                apiKey,
                domain,
                emailFrom,
                region,
                _mailgunEmailOptions.ReplyTo,
                cancellationToken);
        }
    }
}
