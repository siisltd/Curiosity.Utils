using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Curiosity.Tools.Web.SiteMap
{
    /// <summary>
    /// Site map builder.
    /// </summary>
    public class SiteMapBuilder
    {
        private readonly IActionDescriptorCollectionProvider _provider;
        private readonly IWebAppConfigurationWithPublicDomain _webAppConfiguration;
        private readonly SiteMapExtraPageProvider _siteMapExtraPageProvider;

        public SiteMapBuilder(
            IActionDescriptorCollectionProvider provider,
            IWebAppConfigurationWithPublicDomain webAppConfiguration,
            SiteMapExtraPageProvider siteMapExtraPageProvider)
        {
            _provider = provider ?? throw new ArgumentNullException(nameof(provider));
            _webAppConfiguration = webAppConfiguration ?? throw new ArgumentNullException(nameof(webAppConfiguration));
            _siteMapExtraPageProvider = siteMapExtraPageProvider ?? throw new ArgumentNullException(nameof(siteMapExtraPageProvider));
        }

        /// <summary>
        /// Builds response with site map xml data.
        /// </summary>
        /// <param name="httpContext"></param>
        public void BuildSiteMap(HttpContext httpContext)
        {
            var fi = new FileInfo(Assembly.GetExecutingAssembly().Location);
            
            // build routes 
            var uniqueRoutes = new HashSet<string>
            {
                _webAppConfiguration.PublicDomain
            };
            
            foreach (var actionDescriptor in _provider.ActionDescriptors.Items)
            {
                // only http get requests
                var getAttribute = actionDescriptor.EndpointMetadata.OfType<HttpGetAttribute>();
                if (!getAttribute.Any()) continue;
                
                // skip adding to index action with attributes
                var skipSeoAttribute = actionDescriptor.EndpointMetadata.OfType<SkipSeoIndexing>();
                if (skipSeoAttribute.Any()) continue;
                
                if (actionDescriptor.AttributeRouteInfo != null)
                {
                    uniqueRoutes.Add(Path.Combine(_webAppConfiguration.PublicDomain, actionDescriptor.AttributeRouteInfo.Template.TrimStart('/')));
                }
            }
            
            // add extra pages
            foreach (var (url, lastChangeData) in _siteMapExtraPageProvider.GetExtraPages())
            {
                uniqueRoutes.Add(Path.Combine(_webAppConfiguration.PublicDomain, url));
            }
            
            // write xml
            
            // enable sync io for request
            var syncIoFeature = httpContext.Features.Get<IHttpBodyControlFeature>();
            if (syncIoFeature != null)
            {
                syncIoFeature.AllowSynchronousIO = true;
            }

            var response = httpContext.Response;
            
            response.ContentType = "application/xml";
            using (var xml = XmlWriter.Create(response.Body, new XmlWriterSettings { Indent = true }))
            {
                xml.WriteStartDocument();
                xml.WriteStartElement("urlset", "http://www.sitemaps.org/schemas/sitemap/0.9");

                foreach (var route in uniqueRoutes)
                {
                    xml.WriteStartElement("url");
                    xml.WriteElementString("loc", route);
                    xml.WriteElementString("lastmod", fi.CreationTimeUtc.ToString("yyyy-MM-dd"));
                    xml.WriteElementString("changefreq", "daily");
                    xml.WriteEndElement();
                }

                xml.WriteEndElement();
            }
        }
    }
}