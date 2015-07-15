using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Umbraco.Core;
using Umbraco.Core.Models;

using Jumoo.uSync.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.IO;

namespace Jumoo.LocalGovStarterKit
{
    /// <summary>
    ///  Content importer - handles the importing of content as part 
    ///  of the starter kit setup - slightly better than using the 
    ///  packager as it lets us map IDs and the like as part of the import
    ///  
    ///  using uSync.Core to do the hard work...
    /// </summary>
    public class Importer
    {
        private uSyncEngine _uSync;
        public Importer()
        {
            _uSync = new uSyncEngine();
        }

        public bool ImportContentTree(string fileName)
        {
            LogHelper.Info<Importer>("Importing {0}", () => fileName);

            string filePath = IOHelper.MapPath("~/app_data/lgsk.imports/" + fileName);

            if (!System.IO.File.Exists(filePath))
                return false;

            var node = XElement.Load(filePath);

            if (node == null)
                return false;

            LogHelper.Info<Importer>("Loaded XML for Import");

            // imports a whole lot of content from a site.
            var rootGuid = node.Attribute("guid");
            if (rootGuid == null)
            {
                LogHelper.Info<Importer>("No Guid on Node {0}", () => node.Name.LocalName);
                return false;
            }

            if (HasImported(rootGuid.Value))
                return true;

            LogHelper.Info<Importer>("Importing to : {0}", () => rootGuid);

            // ok import this tree (using uSync)
            var _contentService = ApplicationContext.Current.Services.ContentService;
            var root = _contentService.GetRootContent();

            if (root.Any() )
            {
                var homenode = root.Where(x => x.Name == node.Name.LocalName).FirstOrDefault();

                if (homenode != null)
                {
                    // set the root guid to equal the existing one, that way uSync will think
                    // that this is the same node, and import into this tree.
                    node.Attribute("guid").Value = homenode.Key.ToString();
                }
            }

            ImportContentNode(node, -1);
            
            // we actually want IDs in our content to map, and uSync can do that. 
            // but because we might be mapping to content we haven't yet imported...
            
            
            // We import twice
            ImportContentNode(node, -1); // clumsy - but effective. 

            return true;

        }

        private void ImportContentNode(XElement node, int parent)
        {
            LogHelper.Info<Importer>("Importing: {0} - {1}", () => node.Name.ToString(), () => parent);
            var content = _uSync.Content.DeSerialize(node, parent, true);

            if (content != null)
            {
                if (node.Element("Children") != null)
                {
                    var children = node.Element("Children");

                    foreach(var child in children.Elements())
                    {
                        if (child.Name != "Children")
                            ImportContentNode(child, content.Id);
                    }
                }
            }
        }

        /// <summary>
        ///  used in the createion of the kit - actuall makes the export files...
        /// </summary>
        public void ExportContentTree(int nodeId, string filename)
        {
            var _contentService = ApplicationContext.Current.Services.ContentService;

            var node = _contentService.GetById(nodeId);

            if ( node != null)
            {
                XElement contentTree = ExportContent(node);

                var savePath = IOHelper.MapPath("~/app_data/lgsk.imports/" + filename);

                if (System.IO.File.Exists(savePath))
                    System.IO.File.Delete(savePath);

                contentTree.Save(savePath);
            }
        }

        private XElement ExportContent(IContent item)
        {
            var node = _uSync.Content.Serialize(item);

            if (item.Children().Any())
            {
                var childrenNode = new XElement("Children");

                foreach(var child in item.Children())
                {
                    var childNode = ExportContent(child);
                    childrenNode.Add(childNode);
                }

                node.Add(childrenNode);
            }

            return node; 
            
        }

        // does some checking 
        // to see if we have already imported this content
        // stops double imports. 
        public bool HasImported(string importKey)
        {
            return false; 
        }
    }
}
