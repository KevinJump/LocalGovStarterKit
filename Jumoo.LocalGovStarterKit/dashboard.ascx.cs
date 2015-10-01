using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Jumoo.LocalGovStarterKit
{
    public partial class dashboard : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        protected void btnAddExampleContent_Click(object sender, EventArgs e)
        {
            Importer z = new Importer();
            // z.ExportContentTree(1068, "HelpandExample.content");
            z.ImportContentTree("HelpandExample.content");

            importMessage.Text = "Example Content Imported - You should see this on the content node.";
        }

        protected void btnImportLGNL_Click(object sender, EventArgs e)
        {
            esdImporter x = new esdImporter();
            var importCount = x.Import("navigation_englishAndWelshServices.xml", false);
            importMessage.Text = string.Format("Imported {0} Items - go look at the content node", importCount);

        }

        protected void btnImportSNL_Click(object sender, EventArgs e)
        {
            esdImporter x = new esdImporter();
            var importCount = x.Import("navigation_scottishServices.xml", false);
            importMessage.Text = string.Format("Imported {0} Items - go look at the content node", importCount);
        }

        protected void btnTest_Click(object sender, EventArgs e)
        {
            Importer z = new Importer();
            z.ExportContentTree(1068, "HelpandExample.content");
        }

        protected void btnCreateExport_Click(object sender, EventArgs e)
        {
            Importer z = new Importer();
            z.ExportContentTree(1068, "HelpandExample.content");
        }
    }
}