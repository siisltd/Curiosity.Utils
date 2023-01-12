using System;
using Curiosity.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Curiosity.EMail.Mailgun
{
    public static class IocExtensions
    {
        /// <summary>
        /// Adds Mailgun Email sender to services.
        /// </summary>
        public static IServiceCollection AddMailgunEmailSender(
            this IServiceCollection service,
            MailgunEmailOptions options,
            bool useAsDefaultSender = true)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            if (options == null) throw new ArgumentNullException(nameof(options));
            
            // check options
            options.AssertValid();

            service.TryAddSingleton(options);
            service.TryAddSingleton<IMailgunEmailSender, MailgunEmailSender>();
            if (useAsDefaultSender)
            {
                service.TryAddSingleton<IEMailSender>(c => c.GetRequiredService<IMailgunEmailSender>());
            }

            return service;
        }
    }
}
