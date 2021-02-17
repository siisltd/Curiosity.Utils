using System;
using Curiosity.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Curiosity.EMail.Mailgun
{
    public static class IocExtensions
    {
        /// <summary>
        /// Adds Mailgun EMail sender to services.
        /// </summary>
        public static IServiceCollection AddMailgunEMailSender(
            this IServiceCollection service,
            MailgunEMailOptions options,
            bool useAsDefaultSender = true)
        {
            if (service == null) throw new ArgumentNullException(nameof(service));
            if (options == null) throw new ArgumentNullException(nameof(options));
            
            // check options
            options.AssertValid();

            service.TryAddSingleton(options);
            service.TryAddSingleton<IMailgunEMailSender, MailgunEmailSender>();
            if (useAsDefaultSender)
            {
                service.TryAddSingleton<IEMailSender>(c => c.GetRequiredService<IMailgunEMailSender>());
            }

            return service;
        }
    }
}