using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Caching;
using Umbraco.Core;
using Umbraco.Core.Events;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Publishing;
using Umbraco.Core.Services;
using Umbraco.Core.Cache;
using Umbraco.Web;


namespace Jumoo.LocalGov
{
    /// <summary>
    ///  Manages and Builds an AtoZ Cache, that makes the rendering of a sitewide
    ///  AtoZ much quicker and nicer. 
    /// </summary>
    public class AtoZEventHelper : ApplicationEventHandler 
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            ContentService.Published += ContentServicePublishedEvent;
            ContentService.UnPublished += ContentServicePublishedEvent;
        }

        void ContentServicePublishedEvent(IPublishingStrategy sender, PublishEventArgs<IContent> e)
        {
            // Clear all AtoZs

            // could be cleverer - the cache has the root ID in it, so you could just clear caches where
            // the ID is a parent of the current page - not sure it makes much diffrence, because the
            // rebuild only happens once, and it doesn't take 'that' long to rebuild
            ApplicationContext.Current.ApplicationCache.RuntimeCache.ClearCacheByKeySearch("AtoZPages");           
        }
    }

    public static class AtoZHelper
    {
        private static SortedDictionary<string, AtoZInfo> GetAtoZPages(UmbracoHelper umbHelper, string[] excludedTypes)
        {
            var root = umbHelper.TypedContentAtRoot().First();
            return GetAtoZPages(umbHelper, excludedTypes, root);
        }

        private static SortedDictionary<string, AtoZInfo> GetAtoZPages(UmbracoHelper umbHelper, string[] excludedTypes, IPublishedContent root)
        {
            var cacheName = string.Format("AtoZPages_{0}", root.Id);

            var appCache = ApplicationContext.Current.ApplicationCache.RuntimeCache;

            SortedDictionary<string, AtoZInfo> azPages = appCache.GetCacheItem<SortedDictionary<string, AtoZInfo>>(cacheName);

            if (azPages == null)
            {
                Stopwatch sw = new Stopwatch();
                sw.Start();

                // no cache - so we have to build it...
                LogHelper.Debug<AtoZEventHelper>("Building AtoZ Cache");

                azPages = new SortedDictionary<string, AtoZInfo>();

                var sitePages = root.Descendants().Where(
                    x => x.IsVisible()
                    && !excludedTypes.Contains(x.DocumentTypeAlias)
                    && !x.GetPropertyValue<bool>("excludeFromAtoZSearch")
                    && !x.HasProperty("isComponent"));

                foreach(var page in sitePages)
                {
                    var title = page.GetPropertyValue<string>("title", page.Name).Trim();
                    if (!azPages.ContainsKey(title))
                    {
                        azPages.Add(title, new AtoZInfo()
                            {
                                Url = page.Url,
                                Id = page.Id,
                                Title = title
                            });
                    }
                }

                appCache.InsertCacheItem <SortedDictionary<string, AtoZInfo>>
                    (cacheName, () => azPages, priority: CacheItemPriority.Default);

                sw.Stop();

                LogHelper.Info<AtoZEventHelper>("Built atoz cache for {0} pages in {1}ms",
                    () => azPages.Count(), () => sw.ElapsedMilliseconds);
            }
            return azPages;
        }


        public static SortedDictionary<string, AtoZInfo> GetAtoZEntries(this UmbracoHelper umbraco, string letter)
        {
            return GetAtoZEntries(umbraco, letter, new string[] { });
        }

        public static SortedDictionary<string, AtoZInfo> GetAtoZEntries(this UmbracoHelper umbraco, string letter, string[] exclude)
        {
            var root = umbraco.TypedContentAtRoot().First();
            return GetAtoZEntries(umbraco, letter, exclude, root);
        }

        public static SortedDictionary<string, AtoZInfo> GetAtoZEntries(this UmbracoHelper umbraco, string letter, string[] exclude, IPublishedContent root)
        {
            SortedDictionary<string, AtoZInfo> AtoZ = GetAtoZPages(umbraco, exclude, root);

            var sorted = new SortedDictionary<string, AtoZInfo>();
            var entries = AtoZ.Where(x => x.Key.ToLower().StartsWith(letter.ToLower()));
            entries.ForEach(x => sorted.Add(x.Key, x.Value));

            return sorted;
        }
    }

    public class AtoZInfo
    {
        public string Title { get; set; }
        public int Id { get; set; }
        public string Url { get; set; }
    }
}
