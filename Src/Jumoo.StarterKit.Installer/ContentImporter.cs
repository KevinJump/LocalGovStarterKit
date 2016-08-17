using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Xml.Linq;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Core.Logging;

namespace Jumoo.StarterKit.Installer
{
    public class ContentImporter
    {
        const string importPath = "~/app_data/lgsk/";

        public int ImportContent(string fileName)
        {
            int count = 0;
            string importFile = IOHelper.MapPath(importPath + fileName);
            if (!System.IO.File.Exists(importFile))
                return count;

            var node = XElement.Load(importFile);
            if (node == null)
                return count;

            var rootGuid = node.Attribute("guid");
            if (rootGuid == null)
                return count;

            var _contentService = ApplicationContext.Current.Services.ContentService;
            var root = _contentService.GetRootContent();

            if (root.Any())
            {
                var home = root.Where(x => x.Name == node.Name.LocalName).FirstOrDefault();
                if (home != null)
                {
                    node.Attribute("guid").Value = home.Key.ToString();
                }
            }

            count += ImportXmlContent(node, -1);
            MapXmlContent(node, -1);

            return count;

        }

         #region content Imports
        private int ImportXmlContent(XElement node, int parent)
        {
            int count = 0;

            // import this node
            int nodeId = ImportNode(node, parent);

            if (nodeId != -1 && node.Element("Children") != null)
            {
                count++;
                var children = node.Element("Children");
                foreach (var child in children.Elements())
                {
                    if (child.Name != "Children")
                        count += ImportXmlContent(child, nodeId);
                }
            }

            return count; 
        }

        internal IContent GetNode(IContentService _contentService, XElement node)
        {
            var guidNode = node.Attribute("guid");
            if (guidNode == null)
                return null;

            Guid guid = new Guid(guidNode.Value);

            var item = _contentService.GetById(guid);
            return item;
        }

        private int ImportNode(XElement node, int parent)
        {
            var _contentService = ApplicationContext.Current.Services.ContentService;

            var item = GetNode(_contentService, node);
            if (item != null)
                return item.Id;

            var guidNode = node.Attribute("guid");
            if (guidNode == null)
                return -1;

            Guid guid = new Guid(guidNode.Value);

            var name = node.Attribute("nodeName").Value;
            var type = node.Attribute("nodeTypeAlias").Value;
            var templateAlias = node.Attribute("templateAlias").Value;
            var sortOrder = int.Parse(node.Attribute("sortOrder").Value);

            item = _contentService.CreateContent(name, parent, type);
            if (item == null)
                return -1;

            if (item.Trashed)
                item.ChangeTrashedState(false);

            var template = ApplicationContext.Current.Services.FileService.GetTemplate(templateAlias);
            if (template != null)
                item.Template = template;

            item.Key = guid;
            item.SortOrder = sortOrder;
            item.Name = name;

            if (item.ParentId != parent)
                item.ParentId = parent;

            var properties = node.Elements().Where(x => x.Attribute("isDoc") == null);
            ImportProperties(item, properties, false);
            _contentService.Save(item, 0, false);

            return item.Id;
        }

        private void ImportProperties(IContent item, IEnumerable<XElement> properties, bool map = false)
        {
            foreach (var property in properties)
            {
                var propTypeAlias = property.Name.LocalName;
                if (item.HasProperty(propTypeAlias))
                {
                    var propType = item.Properties[propTypeAlias].PropertyType;
                    var value = GetImportXml(property);

                    if (map)
                    {
                        value = MapGuids(value);
                    }
                    
                    try
                    {
                        item.SetValue(propTypeAlias, value);
                    }
                    catch (InvalidOperationException ex)
                    {
                        // Some properties try to store a GUID in property marked as type int
                        // this happens before the GUIDs are mapped back to an int Id
                        // ignore the exception that Umbraco throws as of version 7.5.0
                        LogHelper.Info<ContentImporter>(string.Format("Setting a value didn't work. Tried to set value '{0}' to the property '{1}' on content type alias '{2}'. Exception: {3} {4}", 
                            value, propTypeAlias, item.ContentType.Alias, ex.Message, ex.StackTrace));
                    }
                }
            }
        }

        private string GetImportXml(XElement parent)
        {
            var reader = parent.CreateReader();
            reader.MoveToContent();
            string xml = reader.ReadInnerXml();

            if (xml.StartsWith("<![CDATA["))
                return parent.Value;
            else
                return xml.Replace("&amp;", "&");
        }
        #endregion

        #region 2nd Pass Mapping
        private void MapXmlContent(XElement node, int parent)
        {
            int nodeId = MapNode(node, parent);
            if (nodeId != -1 && node.Element("Children") != null)
            {
                var children = node.Element("Children");
                foreach (var child in children.Elements())
                {
                    if (child.Name != "Children")
                        MapXmlContent(child, nodeId);
                }
            }

        }

        private int MapNode(XElement node, int parent)
        {
            var _contentService = ApplicationContext.Current.Services.ContentService;
            var item = GetNode(_contentService, node);
            if (item == null)
                return -1;

            var properties = node.Elements().Where(x => x.Attribute("isDoc") == null);
            ImportProperties(item, properties, true);

            var published = bool.Parse(node.Attribute("published").Value);

            if (published)
            {
                _contentService.SaveAndPublishWithStatus(item);
            }
            else
            {
                _contentService.Save(item, 0, false);
            }

            return item.Id;
        }

        /// <summary>
        ///  this is a blut guid to id mapper, lifted from usync core, which does this
        ///  in a much more nuanced way - but as we are only importing our sample stuff
        ///  we can't be reasonably confident, the guids are ids.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        private string MapGuids(string value)
        {
            Dictionary<string, string> replacements = new Dictionary<string, string>();

            string guidRegEx = @"\b[A-Fa-f0-9]{8}(?:-[A-Fa-f0-9]{4}){3}-[A-Fa-f0-9]{12}\b";

            foreach (Match m in Regex.Matches(value, guidRegEx))
            {
                var id = GetIdFromGuid(Guid.Parse(m.Value));

                if ((id != -1) && (!replacements.ContainsKey(m.Value)))
                {
                    replacements.Add(m.Value, id.ToString());
                }
            }

            foreach (KeyValuePair<string, string> pair in replacements)
            {
                value = value.Replace(pair.Key, pair.Value);
            }

            return value;
        }

        internal int GetIdFromGuid(Guid guid)
        {
            var item = ApplicationContext.Current.Services.EntityService.GetByKey(guid);
            if (item != null)
                return item.Id;

            return -1;
        }

        #endregion

    }
}
