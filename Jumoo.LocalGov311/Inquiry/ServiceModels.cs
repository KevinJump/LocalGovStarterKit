using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Jumoo.LocalGov311.Inquiry
{
    [XmlRoot("services", Namespace ="")]
    public class ServiceList : List<ServiceItem> { }

    [XmlType("service")]
    public class ServiceItem
    {
        [XmlElement("id")]
        public string Id { get; set; }

        [XmlElement("esd_id")]
        public string EsdId { get; set; }
        public bool ShouldSerializeEsdId()
        {
            return !string.IsNullOrEmpty(EsdId);
        }

        [XmlElement("service_name")]
        public string Title { get; set; }

        [XmlElement("brief_description")]
        public string Summary { get; set; }

        [XmlElement("description")]
        public string Description { get; set; }

        public bool ShouldSerializeDescription()
        {
            return !string.IsNullOrEmpty(Description);
        }

        [XmlElement("modified")]
        public DateTime Modified { get; set; }

        [XmlElement("expiration")]
        public DateTime Expiration { get; set; }

        [XmlElement("url")]
        public string Url { get; set; }

    }
}
