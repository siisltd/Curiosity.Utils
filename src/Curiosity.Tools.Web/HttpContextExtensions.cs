using System;
using System.Linq;
using System.Net;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Routing;

namespace Curiosity.Tools.Web
{
    public static class HttpContextExtensions
    {
        private static readonly char[] Delimeters = {' ', ',', ';'};

        public static string GetAbsoluteUrl(this HttpContext context, string relativeUrl)
        {
            return $"{context.Request.Scheme}://{context.Request.Host}{relativeUrl}";
        }
        
        public static bool IsAjaxRequest(this HttpContext context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            if (context.Request == null) throw new ArgumentNullException(nameof(context.Request));
            
            return context.Request.IsAjaxRequest();
        }
        
        public static bool IsAjaxRequest(this HttpRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            
            if (request.Headers != null)
                return request.Headers["X-Requested-With"] == "XMLHttpRequest";
            
            return false;
        }
        
        public static string GetUserAgent(this HttpRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            
            return request.Headers["User-Agent"].ToString();
        }
        
        public static string GetUserHost(this HttpRequest request)
        {
            if (request == null) throw new ArgumentNullException(nameof(request));
            
            return request.HttpContext.Connection.RemoteIpAddress.ToString();
        }

        public static bool IsLocalRequest(this HttpContext context)
        {
            return context.Connection.RemoteIpAddress.Equals(context.Connection.LocalIpAddress) ||
                   IPAddress.IsLoopback(context.Connection.RemoteIpAddress);
        }
        
        public static bool IsControllerAction(this HttpContext context, string action, string controller)
        {
            var routeDataController = context.GetRouteData().Values["controller"]?.ToString();
            var routeDataAction = context.GetRouteData().Values["action"]?.ToString();

            if (routeDataController == null || routeDataAction == null)
                return false;

            return
                controller.Equals(routeDataController, StringComparison.OrdinalIgnoreCase) &&
                action.Equals(routeDataAction, StringComparison.OrdinalIgnoreCase);
        }

        public static bool IsControllerActions(this HttpContext context, string actions, string controller)
        {
            var routeDataController = context.GetRouteData().Values["controller"]?.ToString();
            var routeDataAction = context.GetRouteData().Values["action"]?.ToString();

            if (routeDataController == null || routeDataAction == null)
                return false;

            return controller.Equals(routeDataController, StringComparison.OrdinalIgnoreCase) &&
                   actions
                       .Split(Delimeters, StringSplitOptions.RemoveEmptyEntries)
                       .Any(action => action.Equals(routeDataAction, StringComparison.OrdinalIgnoreCase));
        }

        public static bool IsController(this HttpContext context, string controller)
        {
            var routeController = context.GetRouteData().Values["controller"]?.ToString();
            return routeController != null && controller.Equals(routeController, StringComparison.OrdinalIgnoreCase);
        }

        public static string RequestUrlBase(this HttpContext context)
        {
            var url = new Uri(context.Request.GetDisplayUrl());
            return url.GetLeftPart(UriPartial.Authority);
        }
        
        /// <summary>
        /// Determines whether the request comes from a bot/service that makes a preview of the page (for example, in Telegram, WhatsApp, Google, etc.).
        /// </summary>
        /// <remarks>
        /// User agents taken from here https://developers.whatismybrowser.com/useragents/explore/software_name/
        /// and from here: https://udger.com/resources/ua-list
        /// </remarks>
        public static bool IsPreviewRequest(this HttpContext context)
        {
            var userAgent = context.Request.GetUserAgent().ToUpper();
            
            // many bots have many useragents that differ only in version (e.g. WhatsApp/2.19.333 A, e.g. WhatsApp/2.19.333 I)
            // so just search by occurrence

            // using useragents specifically bots or preview creators
            // because many applications have their own built-in browsers and there will also be mentions of the application
            
            // WhatsApp: user agent bots start with this text
            if (userAgent.StartsWith("WHATSAPP")) return true;
            
            // Telegram
            if (userAgent.Contains("TELEGRAMBOT")) return true;
            
            // ВК
            if (userAgent.Contains("VKSHARE")) return true;
            
            // у поисковых работов Яндекса - `YandexBot`
            if (userAgent.Contains("YANDEXBOT")) return true;
            
            // боты Гугла
            if (userAgent.Contains("GOOGLEBOT")) return true;
            
            // боты FaceBook
            if (userAgent.Contains("FACEBOOKEXTERNALHIT") || userAgent.Contains("FACEBOT")) return true;
            
            // Bing - BingPreview
            if (userAgent.Contains("BINGPREVIEW")) return true;

            // Zoom Info Bot
            if (userAgent.Contains("ZOOMINFOBOT")) return true;
            
            // Discord
            if (userAgent.Contains("DISCORDBOT")) return true;
            
            // Skype
            if (userAgent.Contains("SKYPEURIPREVIEW")) return true;
            
            // LinkedIn
            if (userAgent.Contains("LINKEDINBOT")) return true;
            
            // Одноклассники
            if (userAgent.Contains("ODKLBOT")) return true;
            
            // Twitter
            if (userAgent.Contains("TWITTERMBOT")) return true;
            
            // Mail.ru
            if (userAgent.Contains("MAIL.RU BOT")) return true;
            
            // Outlook
            if (userAgent.Contains("OUTLOOK")) return true;
            
            // Thunderbird
            if (userAgent.Contains("THUNDERBIRD")) return true;
            
            // Yahoo
            if (userAgent.Contains("YAHOO LINK PREVIEW")) return true;
            
            return false;
        }
    }
}