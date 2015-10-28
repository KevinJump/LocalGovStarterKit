using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;

using Umbraco.Core;
using Umbraco.Core.Cache;

namespace Jumoo.LocalGov311
{
    [PluginController("Open311")]
    public class Open311SettingsApiController : UmbracoAuthorizedJsonController
    {
        public Open311Settings GetSettings()
        {
            return Open311Settings.Current;
        }

        public string GetApiRoot()
        {
            return Open311Settings.Current.ApiRootUrl;
        }

        public int GetCacheSize()
        {
            var cache = ApplicationContext.Current.ApplicationCache.RuntimeCache;
            ServiceList serviceList = cache.GetCacheItem<ServiceList>(Open311Settings.Current.CacheName);

            if (serviceList != null && serviceList.Any())
            {
                return serviceList.Count;
            }

            return 0;
        }
    }
}
