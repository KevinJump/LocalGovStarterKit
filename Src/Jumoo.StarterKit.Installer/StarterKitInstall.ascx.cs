using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Jumoo.StarterKit.Installer
{
    public partial class StarterKitInstall : System.Web.UI.UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                if (Request.QueryString["installing"] != null)
                {
                    googletag.Visible = true; 
                }
                else
                {
                    contentWarning.Visible = true; 
                }
            }
            else
            {
                googletag.Visible = false; 
            }
        }

        /// <summary>
        ///  importers. 
        /// </summary>
        protected void btnExampleImport_Click(object sender, EventArgs e)
        {
            var importer = new ContentImporter();
            var result = importer.ImportContent("example.content");
            divResults.Visible = true; 
            lbResults.Text = string.Format("Example Content Import Complete: {0} items created", result);
        }

        protected void btnLgnlImport_Click(object sender, EventArgs e)
        {
            esdImporter importer = new esdImporter();
            var result = importer.Import("navigation_englishAndWelshServices.xml", false);
            divResults.Visible = true;
            lbResults.Text = string.Format("Imported LGNL Structure: {0} items", result);
        }

        protected void btnSnlImport_Click(object sender, EventArgs e)
        {
            esdImporter importer = new esdImporter();
            var result = importer.Import("navigation_scottishServices.xml", false);
            divResults.Visible = true;
            lbResults.Text = string.Format("Imported SNL Structure: {0} items", result);
        }
    }
}