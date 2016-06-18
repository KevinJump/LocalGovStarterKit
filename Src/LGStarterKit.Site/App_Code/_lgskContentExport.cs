using Jumoo.uSync.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.Models;
using Umbraco.Web.Mvc;
using Umbraco.Web.WebApi;

namespace LGStarterKit
{
    /// <summary>
    ///  export code - this isn't included in the package, 
    ///  it's seperate because of it's use of usync
    ///  
    /// </summary>
    /// 
    public class ContentExportController : UmbracoAuthorizedApiController
    { 
        const string importPath = "~/app_data/lgsk/";

        /// <summary>
        ///  Generates teh exmaple content export
        ///  http://localhost:56520/umbraco/backoffice/api/ContentExport/GetContent?nodeId=1136
        /// </summary>
        /// <param name="nodeId"></param>
        /// <returns></returns>
        public bool GetContent(int nodeId)
        {
            uSyncCoreContext.Instance.Init();

            var node = ApplicationContext.Current.Services.ContentService.GetById(nodeId);

            if (node != null)
            {
                XElement contentTree = ExportItem(node);

                var saveFile = IOHelper.MapPath(importPath + "example.content");

                if (System.IO.File.Exists(saveFile))
                {
                    System.IO.File.Delete(saveFile);
                }

                contentTree.Save(saveFile);
            }

            return true;
        }

        private XElement ExportItem(IContent item)
        {
            var attempt = uSyncCoreContext.Instance.ContentSerializer.Serialize(item);

            if (attempt.Success)
            {
                var node = attempt.Item;

                if (item.Children().Any())
                {
                    var childrenNode = new XElement("Children");

                    foreach (var child in item.Children())
                    {
                        var childNode = ExportItem(child);
                        childrenNode.Add(childNode);
                    }
                    node.Add(childrenNode);
                }
                return node;
            }

            return null;
        }


    }
}