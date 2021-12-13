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
    public class MailgunEmailSender : IMailgunEMailSender
    {
        private readonly ILogger<MailgunEmailSender> _logger;
        private readonly MailgunEMailOptions _mailgunEMailOptions;

        public MailgunEmailSender(
            ILogger<MailgunEmailSender> logger,
            MailgunEMailOptions mailgunEMailOptions)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));

            _mailgunEMailOptions = mailgunEMailOptions ?? throw new ArgumentNullException(nameof(mailgunEMailOptions));
            _mailgunEMailOptions.AssertValid();
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
            if (String.IsNullOrWhiteSpace(toAddress)) throw new ArgumentNullException(nameof(toAddress));
            if (String.IsNullOrWhiteSpace(subject)) throw new ArgumentNullException(nameof(subject));
            if (String.IsNullOrWhiteSpace(body)) throw new ArgumentNullException(nameof(body));

            return SendAsync(
                toAddress,
                subject,
                body,
                isBodyHtml,
                _mailgunEMailOptions.MailgunUser,
                _mailgunEMailOptions.MailGunApiKey,
                _mailgunEMailOptions.MailGunDomain,
                _mailgunEMailOptions.EMailFrom,
                _mailgunEMailOptions.MailgunRegion,
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

            var restClient = new RestClient
            {
                BaseUrl = new Uri(mailgunHost),
                Authenticator = new HttpBasicAuthenticator(mailgunUser, mailGunApiKey)
            };

            var restRequest = new RestRequest();
            restRequest.AddParameter("domain", mailgunDomain, ParameterType.UrlSegment);
            restRequest.Resource = "{domain}/messages";
            restRequest.AddParameter("from", emailFrom);
            restRequest.AddParameter("to", toAddress);
            restRequest.AddParameter("subject", subject);
            restRequest.AddParameter(isBodyHtml ? "html" : "text", body);
            restRequest.Method = Method.POST;

            _logger.LogTrace("Sending email to {Email}...", toAddress);
            var response = await restClient.ExecuteAsync(restRequest, cancellationToken);
            if (!response.IsSuccessful)
            {
                _logger.LogError($"Error sending message to {toAddress}. StatusCode = {response.StatusCode.ToString()}. Response: {response.Content}");

                return Response.Failed(new Error((int)EmailError.Auth, response.Content));
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

        public Task<Response> SendAsync(
            string toAddress,
            string subject,
            string body,
            bool isBodyHtml,
            IEMailExtraParams emailExtraParams,
            CancellationToken cancellationToken = default)
        {
            if (String.IsNullOrWhiteSpace(toAddress)) throw new ArgumentNullException(nameof(toAddress));
            if (String.IsNullOrWhiteSpace(subject)) throw new ArgumentNullException(nameof(subject));
            if (String.IsNullOrWhiteSpace(body)) throw new ArgumentNullException(nameof(body));
            if (emailExtraParams == null) throw new ArgumentNullException(nameof(emailExtraParams));

            if (!(emailExtraParams is MailGunEMailExtraParams mailGunEMailExtraParams))
                throw new ArgumentException($"Only {typeof(MailGunEMailExtraParams)} is supported for this sender.", nameof(emailExtraParams));

            var user = mailGunEMailExtraParams.MailgunUser ?? _mailgunEMailOptions.MailgunUser;
            var apiKey = mailGunEMailExtraParams.MailGunApiKey ?? _mailgunEMailOptions.MailGunApiKey;
            var domain = mailGunEMailExtraParams.MailGunDomain ?? _mailgunEMailOptions.MailGunDomain;
            var emailFrom = mailGunEMailExtraParams.EMailFrom ?? _mailgunEMailOptions.EMailFrom;
            var region = mailGunEMailExtraParams.MailgunRegion ?? _mailgunEMailOptions.MailgunRegion;

            return SendAsync(toAddress, subject, body, isBodyHtml, user, apiKey, domain, emailFrom, region, cancellationToken);
        }
    }
}
