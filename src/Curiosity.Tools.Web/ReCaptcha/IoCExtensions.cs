using System;
using Microsoft.Extensions.DependencyInjection;

namespace Curiosity.Tools.Web.ReCaptcha
{
    public static class IoCExtensions
    {
        public static void AddReCaptcha(this IServiceCollection services, ReCaptchaOptions reCaptchaOptions)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));
            if (reCaptchaOptions == null) throw new ArgumentNullException(nameof(reCaptchaOptions));

            services.AddSingleton(reCaptchaOptions);
            services.AddSingleton<ReCaptchaService>();
        }
    }
}