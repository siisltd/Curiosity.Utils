using System;
using System.Net;
using Ganss.XSS;

namespace Curiosity.Tools.Web
{
    public static class HtmlSanitizerHelper
    {
        private static readonly HtmlSanitizer HtmlSanitizer;

        static HtmlSanitizerHelper()
        {
            HtmlSanitizer = new HtmlSanitizer();

            HtmlSanitizer.AllowedTags.Add("audio");
            HtmlSanitizer.AllowedTags.Add("video");
            HtmlSanitizer.AllowedTags.Add("source");

            HtmlSanitizer.AllowedAttributes.Add("preload");
            HtmlSanitizer.AllowedAttributes.Add("controls");
            HtmlSanitizer.AllowedAttributes.Add("autoplay");
            HtmlSanitizer.AllowedAttributes.Add("muted");
            HtmlSanitizer.AllowedAttributes.Add("loop");

            HtmlSanitizer.AllowedAttributes.Add("id");
            HtmlSanitizer.AllowedAttributes.Add("class");

            HtmlSanitizer.AllowedAttributes.Add("face");

            HtmlSanitizer.AllowedSchemes.Add("mailto");
            HtmlSanitizer.AllowedSchemes.Add("sip");
            HtmlSanitizer.AllowedSchemes.Add("tel");
            HtmlSanitizer.AllowedSchemes.Add("skype");
        }
        
        public static string Sanitize(string html)
        {
            var sanitized =  !String.IsNullOrEmpty(html) ? HtmlSanitizer.Sanitize(html) : html;
            
            return WebUtility.HtmlDecode(sanitized);    // decode special chars (&amp и т.д.)
        }
    }
}