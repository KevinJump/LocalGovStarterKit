using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Text;
using System.Threading.Tasks;
using System.Web.Caching;
using System.Xml.Serialization;
using Umbraco.Core;
using Umbraco.Core.Cache;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Web;
using Umbraco.Web.WebApi;

namespace Jumoo.LocalGov311
{
    /// <summary>
    ///  The Inquiry Api for Open311 v1 - using properties inside umbraco we 
    /// build a cached version of all the services, and then when requested we 
    /// throw out a compatible open311 files to show all the things we have.
    /// 
    /// the inquiry api, is just for getting info its not for reports and stuff.
    /// that is in the geoAPI (which is more back office dependent)
    /// 
    /// </summary>
    public class InquiryController : UmbracoApiController
    {

        // {apiroot}/

        /// <summary>
        /// gets all the services.
        /// </summary>
        public HttpResponseMessage Get()
        {
            var services = GetServiceList();

            //
            // we want to use the XmlSerializer - but we can't set that 
            // globally because we will bork umbraco
            // 
            return GetReturnData(services);
        }

        private HttpResponseMessage GetReturnData(ServiceList services)
        {
            XmlMediaTypeFormatter xmlFormatter = new XmlMediaTypeFormatter();
            xmlFormatter.UseXmlSerializer = true;
            var content = new ObjectContent<ServiceList>(services, xmlFormatter);
            return new HttpResponseMessage()
            {
                Content = content
            };
        }


        // {apiroot/id}

        /// <summary>
        ///  gets all the services with the EsdId provided
        /// 
        ///  this call, doesn't call the service cache, it calls
        ///  the umbraco objects and gets all teh info from there
        /// </summary>
        public HttpResponseMessage Get(string id)
        {
            var serviceList = new ServiceList();

            var roots = Umbraco.TypedContentAtRoot();

            foreach(var root in roots)
            {
                IEnumerable<IPublishedContent> nodes = null;
                if (Open311Settings.Current.useEsdAsId)
                {
                    nodes = root.Descendants()
                        .Where(x => x.IsVisible()
                        && x.GetPropertyValue<string>(Open311Settings.Current.Fields.EsdId) == id);
                }
                else
                {
                    nodes = root.Descendants()
                        .Where(x => x.IsVisible() && x.GetKey().ToString() == id);
                }

                if (nodes!= null && nodes.Any())
                {
                    foreach(var node in nodes)
                    {
                        serviceList.Add(GetServiceFromNode(node, true));
                    }
                    break;
                }
            }
            return GetReturnData(serviceList);
        }

        /// <summary>
        ///  gets the service list from either the cache or umbraco.
        /// </summary>
        /// <returns></returns>
        private ServiceList GetServiceList()
        {
            var cache = ApplicationContext.Current.ApplicationCache.RuntimeCache;

            ServiceList serviceList = cache.GetCacheItem<ServiceList>(Open311Settings.Current.CacheName);

            if (serviceList == null)
            {
                serviceList = new ServiceList();

                var siteRoots = Umbraco.TypedContentAtRoot();
                foreach(var root in siteRoots)
                {
                    IEnumerable<IPublishedContent> nodes = null;
                    if (Open311Settings.Current.useEsdAsId)
                    {
                        nodes = root.Descendants()
                            .Where(x => x.IsVisible() && x.HasValue(Open311Settings.Current.Fields.EsdId));
                    }
                    else
                    {
                        nodes = root.Descendants()
                            .Where(x => x.IsVisible());
                    }

                    if (nodes != null && nodes.Any())
                    {
                        foreach (var node in nodes)
                        {
                            serviceList.Add(GetServiceFromNode(node));
                        }
                    }
                }

                cache.InsertCacheItem<ServiceList>(Open311Settings.Current.CacheName,
                    () => serviceList, priority: CacheItemPriority.Default);

            }

            return serviceList;
        }

        private ServiceItem GetServiceFromNode(IPublishedContent node, bool full = false)
        {
            var fields = Open311Settings.Current.Fields;

            var id = Open311Settings.Current.useEsdAsId 
                ? node.GetPropertyValue<string>(fields.EsdId, node.GetKey().ToString()) 
                : node.GetKey().ToString();

            var item = new ServiceItem
            {
                Id = id,
                Title = node.GetPropertyValue<string>(fields.Title, node.Name),
                Summary = node.GetPropertyValue<string>(fields.Summary, "pageSummary"),
                Expiration = node.GetPropertyValue<DateTime>(fields.Expiry, DateTime.Now.AddYears(10)),
                Modified = node.UpdateDate,
                Url = Umbraco.NiceUrlWithDomain(node.Id)
            };

            if (!Open311Settings.Current.useEsdAsId)
            {
                item.EsdId = node.GetPropertyValue<string>(fields.EsdId);
            }

            if (full)
            {
                item.Description = node.GetPropertyValue<string>(fields.Description);
            }

            return item;
        }
    }
}
