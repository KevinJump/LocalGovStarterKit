using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Routing;
using Umbraco.Core;
using Umbraco.Core.Services;

namespace Jumoo.LocalGov311
{
    public class ServiceEventHelper : ApplicationEventHandler
    {
        protected override void ApplicationStarting(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            // some routing stuff, so we can put our api in a nice place.
            var inquiryRoot = string.Format("{0}/{{controller}}/{{id}}", 
                                Open311Settings.Current.ApiRootUrl);

            RouteTable.Routes.MapHttpRoute("Open311Inqury",
                inquiryRoot, new {
                    controller = "Inquiry",
                    id = RouteParameter.Optional,
                    action = "Get"
                });
                
        }

        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            ContentService.Published += ContentServicePublishEvent;
            ContentService.UnPublished += ContentServicePublishEvent;
        }

        // will be called everytime some content is published / unpublished
        // allows us to manage the cache as and when we need to.
        private void ContentServicePublishEvent(Umbraco.Core.Publishing.IPublishingStrategy sender, Umbraco.Core.Events.PublishEventArgs<Umbraco.Core.Models.IContent> e)
        {
            throw new NotImplementedException();
        }
    }
}
