using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Caching;

using Umbraco.Core;
using Umbraco.Core.Models;
using Umbraco.Core.Cache;

using Umbraco.Web;
using Umbraco.Core.Logging;
using System.Diagnostics;
using Umbraco.Core.Services;
using Umbraco.Core.Publishing;
using Umbraco.Core.Events;

namespace LGStarterKit.AtoZ
{
    /// <summary>
    ///  manages the cache on publish/unpublish events, when something is published we 
    ///  clear the cache, the next user to visit the atoz will then trigger a rebuild
    /// </summary>
    public class AtoZCacheEventHelper : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            ContentService.Published += ContentServicePublishedEvent;
            ContentService.UnPublished += ContentServicePublishedEvent;
        }

        private void ContentServicePublishedEvent(IPublishingStrategy sender, PublishEventArgs<IContent> e)
        {
            // clear the atoz's
            // this is the quick way, clear any atoz cache when something is published or unpublished
            // we could at this point find the root of each published item and only clear the root
            // of that atoz - or indeed you could build the atoz on publish, but this would only 
            // slow down publish, for something that is not always used. 

            // even on a biggish site, an atoz can be built in around 1 second
            ApplicationContext.Current.ApplicationCache.RuntimeCache.ClearCacheByKeySearch("AtoZPages");
        }
    }


    /// <summary>
    ///  Manages and builds a AtoZ Cache for site wide or sections of the site
    ///  makes the atoz quicker, and puts a lot less load on the site for what
    ///  is usally a little used bit of functionality. 
    ///  
    ///  this is the basic atoz cache, often you need to add alternate entries
    ///  you can do this just looking for those properties as you loop through
    ///  site pages, and adding them as alternates. 
    ///  
    ///  this implimentation is the starter sample to get you going 
    /// </summary>
    public static class AtoZCacheHelper 
    {
        private static SortedDictionary<string, AtoZInfo> GetAtoZPages(UmbracoHelper umbHelper, string[] excludedTypes)
        {
            var root = umbHelper.TypedContentAtRoot().First();
            return GetAtoZPages(umbHelper, excludedTypes, root);
        }

        private static SortedDictionary<string, AtoZInfo> GetAtoZPages(UmbracoHelper umbHelper, string[] excludedTypees, IPublishedContent root)
        {
            var cacheName = string.Format("AtoZPages_{0}", root.Id);
            var appCache = ApplicationContext.Current.ApplicationCache.RuntimeCache;

            SortedDictionary<string, AtoZInfo> azPages = appCache.GetCacheItem<SortedDictionary<string, AtoZInfo>>(cacheName);

            if (azPages == null)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                // no cache, we need to build it. 
                LogHelper.Debug<AtoZCacheEventHelper>("Building Cache: {0}", () => cacheName);

                azPages = new SortedDictionary<string, AtoZInfo>();

                var sitePages = root.Descendants().Where(
                                    x => x.IsVisible()
                                    && !excludedTypees.Contains(x.DocumentTypeAlias)
                                    && !x.GetPropertyValue<bool>("excludeFromAtoZ")
                                    && !x.HasProperty("isComponent"));

                foreach(var page in sitePages)
                {
                    var title = page.GetPropertyValue<string>("title", page.Name).Trim();
                    if (!azPages.ContainsKey(title))
                    {
                        azPages.Add(title, new AtoZInfo()
                        {
                            Title = title,
                            id = page.Id,
                            url = page.Url
                        });
                    }
                }

                appCache.InsertCacheItem<SortedDictionary<string, AtoZInfo>>
                    (cacheName, () => azPages, priority: CacheItemPriority.Default);

                sw.Stop();

                LogHelper.Debug<AtoZCacheEventHelper>("Build cache {0} for {1} pages in {2}ms",
                    () => cacheName, () => azPages.Count(), () => sw.ElapsedMilliseconds);
            }

            return azPages;
        }

        public static SortedDictionary<string, AtoZInfo> GetAtoZEntries(this UmbracoHelper umbraco, string letter)
        {
            return GetAtoZEntries(umbraco, letter, new string[] { });
        }

        public static SortedDictionary<string, AtoZInfo> GetAtoZEntries(this UmbracoHelper umbraco, string letter, string[] exludedTypes)
        {
            var root = umbraco.TypedContentAtRoot().First();
            return GetAtoZEntries(umbraco, letter, new string[] { }, root);
        }

        public static SortedDictionary<string, AtoZInfo> GetAtoZEntries(this UmbracoHelper umbraco, string letter, string[] exludedTypes, IPublishedContent root)
        {
            SortedDictionary<string, AtoZInfo> atoz = GetAtoZPages(umbraco, exludedTypes, root);

            var sorted = new SortedDictionary<string, AtoZInfo> ();
            var entries = atoz.Where(x => x.Key.ToLower().StartsWith(letter.ToLower()));
            entries.ForEach(x => sorted.Add(x.Key, x.Value));

            return sorted;
            
        }
    }

    /// <summary>
    ///  info that is stored in the cache. 
    /// </summary>
    public class AtoZInfo
    {
        public string Title { get; set; }
        public int id { get; set; }
        public string url { get; set; }
    }
}