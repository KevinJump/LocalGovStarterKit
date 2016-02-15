using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Linq;
using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.Logging;
using Umbraco.Core.Models;
using Umbraco.Core.Services;

namespace Jumoo.LocalGovStarterKit
{
    /// <summary>
    ///  Importer contstants, (for class below)
    ///  
    ///  Define just where we get the content from, and how 
    ///  we map that content to the site. 
    ///  
    ///  these constants are setup to import a ESD XML file
    /// </summary>
    public static class ImporterConstants
    {
        // pages in our umbraco install
        public const string NavigationTemplate = "GatewayPage";
        public const string ContentTemplate = "ContentPage";

        // properties in our umbraco install.
        public const string ContentTitle = "title";
        public const string ContentBody = "bodyText";
        public const string ContentSummary = "pageSummary";
        public const string ContentEsdId = "esdServiceId";

        // node structure of ESD file, 
        public const string EsdNavigationNode = "WebsiteNavigation";
        public const string EsdServiceListNode = "Services";
        public const string EsdServiceNode = "Service";
        
        // elements in the esd file
        public const string EsdTitleNode = "Label";
        public const string EsdIdNode = "Identifier";
        public const string EsdSummaryNode = "Description";

    }

    /// <summary>
    ///  imports esd xml files in as content, 
    ///  this means you can load the LGNL into your site
    ///  nice and quick.
    /// </summary>
    public class esdImporter
    {
        IContentService _contentService;
        int _importCount = 0;

        public esdImporter()
        {
            _contentService = ApplicationContext.Current.Services.ContentService;
        }

        public int Import(string fileName, bool importServicePages)
        {
            string file = IOHelper.MapPath("~/app_data/lgsk.imports/" + fileName);

            if (!System.IO.File.Exists(file))
                return -1;

            XElement content = XElement.Load(file);

            if (content == null)
                return -1;

            var home = _contentService.GetRootContent().FirstOrDefault();
            if (home == null)
            {
                // no homenode = we create one.
                home = MakeHomeNode();
            }               

            _importCount = 0;

            foreach(var node in content.Elements(ImporterConstants.EsdNavigationNode))
            {
                ImportNode(node, home.Id, true, importServicePages);
            }

            return _importCount;
        }

        public void ImportNode(XElement node, int parentId, bool Navigation, bool doServices)
        {
            int pageId = -1;
            if (Navigation)
            {
                pageId = CreateContent(ImporterConstants.NavigationTemplate, node, parentId);
            }
            else
            {
                if (doServices)
                {
                    pageId = CreateContent(ImporterConstants.ContentTemplate, node, parentId);
                }
            }

            if (pageId != -1)
            {
                if (node.Elements(ImporterConstants.EsdServiceListNode) != null)
                {
                    foreach (var child in node.Elements(ImporterConstants.EsdServiceListNode)
                        .Elements(ImporterConstants.EsdServiceNode))
                    {
                        ImportNode(child, pageId, false, doServices);
                    }
                }

                if (node.Elements(ImporterConstants.EsdNavigationNode) != null)
                {
                    foreach (var child in node.Elements(ImporterConstants.EsdNavigationNode)
                        .Elements(ImporterConstants.EsdNavigationNode))
                    {
                        ImportNode(child, pageId, true, doServices);
                    }
                }
            }
        }

        public int CreateContent(string template, XElement node, int parent)
        {
            var title = node.Element(ImporterConstants.EsdTitleNode).Value;
            var esdId = node.Element(ImporterConstants.EsdIdNode).Value;
            var summary = node.Element(ImporterConstants.EsdSummaryNode).Value;

            var content = _contentService.CreateContent(title, parent, template);
            if (content != null)
            {
                SetContentValue(content, ImporterConstants.ContentTitle, title);
                SetContentValue(content, ImporterConstants.ContentEsdId, esdId);
                SetContentValue(content, ImporterConstants.ContentSummary, summary);

                _contentService.SaveAndPublishWithStatus(content);

                _importCount++; 
                return content.Id;
            }

            return -1; 
        }

        public void SetContentValue(IContent content, string name, string value)
        {
            if (content.HasProperty(name))
            {
                content.SetValue(name, value);
            }
        }

        private IContent MakeHomeNode()
        {
            var home = _contentService.CreateContent("LocalGov Home", -1, "Homepage");

            if (home != null)
            {
                home.SetValue("title", "LocalGov Starter Kit");
                home.SetValue("bodyText", "Welcome to the LocalGov StarterKit for umbraco, have a look around and see how you could use umbraco to build your own site.");
                home.SetValue("siteName", "LocalGov Starterkit<sup>5</sup>");
                home.SetValue("sectionName", "Home");

                _contentService.SaveAndPublishWithStatus(home);

                return home;
            }

            return null;
        }
    }
}