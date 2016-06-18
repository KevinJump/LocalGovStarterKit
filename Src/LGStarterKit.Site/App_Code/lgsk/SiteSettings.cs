using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Umbraco.Core.Models;
using Umbraco.Web;

namespace LGStarterKit.Utils
{
    /// <summary>
    ///  nice simple class that lets you get global settings (from the GlobalSettings node) and use them.
    ///  
    ///  if their is no global settings node, or if the value is missing or not set you can return a
    ///  default value. 
    /// </summary>
    public static class SiteSettings
    {
        public static T GetSiteSetting<T>(this UmbracoHelper umbraco, string alias, T defaultValue)
        {
            var settings = umbraco.TypedContentSingleAtXPath("//globalSettings");

            if (settings != null && settings.HasProperty(alias))
            {
                return settings.GetPropertyValue<T>(alias);
            }

            return defaultValue;
        }

        public static T GetSiteSetting<T>(this UmbracoHelper umbraco, string alias)
        {
            return GetSiteSetting(umbraco, alias, default(T));
        }
    }
}