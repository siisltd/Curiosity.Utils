using System;
using System.Threading.Tasks;
using Flurl;
using Flurl.Http;

namespace Curiosity.Tools.Web.ReCaptcha
{
    public class ReCaptchaService
    {
        private readonly ReCaptchaOptions _options;

        public ReCaptchaService(ReCaptchaOptions options)
        {
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }
        
        public async Task<bool> VerifyReCaptchaAsync(string response)
        {
            if (String.IsNullOrWhiteSpace(response))
                return false;
            
            var reCaptchaResponse = await _options.ReCaptchaApiUrl
                .SetQueryParam("secret", _options.ReCaptchaServerKey)
                .SetQueryParam("response", response)
                .GetJsonAsync<ReCaptchaResponse>();

            return reCaptchaResponse?.success ?? false;
        }
    }
}