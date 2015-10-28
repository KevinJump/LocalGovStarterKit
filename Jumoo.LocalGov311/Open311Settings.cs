using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;

namespace Jumoo.LocalGov311
{
    public class Open311Settings
    {
        private static Object _lock = new object();
        private static bool _loaded = false;

        private static Open311Settings _current;
        public static Open311Settings Current
        {
            get
            {
                if (_current == null)
                    _current = LoadSettings();

                return _current;
            }
        }

        public string ApiRootUrl { get; set; }
        public string CacheName { get; set; }

        public bool useEsdAsId { get; set; }

        public ServiceFields Fields { get; set; }

        public Open311Settings() { }


        /// <summary>
        ///  load and save functions to write the settings out to disk....
        /// </summary>
        /// <returns></returns>
        private static Open311Settings LoadSettings()
        {
            Open311Settings settings = null;
            if (!_loaded)
            {
                lock(_lock)
                {
                    try
                    {
                        var config = IOHelper.MapPath(
                                        Path.Combine(SystemDirectories.Config, "Open311.Config"));

                        if (System.IO.File.Exists(config))
                        {
                            XmlSerializer serializer = new XmlSerializer(typeof(Open311Settings));
                            using (FileStream fs = new FileStream(config, FileMode.Open))
                            {
                                 settings = (Open311Settings)serializer.Deserialize(fs);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        LogHelper.Warn<Open311Settings>("Unable to load Open 311 settings, using defaults {0}", () => ex);
                    }
                }
            }

            if (settings == null)
            {
                // defaults 
                // set defaults 
                settings = new Open311Settings();
                //
                settings.ApiRootUrl = "Open311Api";
                settings.useEsdAsId = true; // if true, then we use the EsdService ID as the id. 

                settings.Fields = new ServiceFields
                {
                    EsdId = "esdServiceId",
                    Description = "bodyText",
                    Summary = "pageSummary",
                    Title = "title",
                    Expiry = "Expiration"
                };

                settings.CacheName = "Open311Inquiry";

                // save the settings...
                SaveSettings(settings);

            }

            return settings;
        }

        private static void SaveSettings(Open311Settings settings)
        {
            var config = IOHelper.MapPath(
                            Path.Combine(SystemDirectories.Config, "Open311.Config"));

            if (System.IO.File.Exists(config))
                File.Delete(config);

            XmlSerializer serializer = new XmlSerializer(typeof(Open311Settings));

            using (StreamWriter w = new StreamWriter(config))
            {
                serializer.Serialize(w, settings);
            }
        }
    }

    public class ServiceFields
    {
        public string EsdId { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; } 
        public string Description { get; set; }
        public string Expiry { get; set; }
    }
}
