using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Http;
using System.Xml.Linq;

using Umbraco.Core;
using Umbraco.Core.IO;
using Umbraco.Core.Models;
using Umbraco.Core.Services;
using Umbraco.Web.Editors;
using Umbraco.Web.Mvc;

namespace LGStarterKit.Dashboard
{
    [PluginController("lgsk")]
    public class lgskImporter
    {
        const string importPath = "~/app_data/lgsk/";

        [HttpGet]
        public IEnumerable<ImportStructure> GetImports()
        {
            List<ImportStructure> structures = new List<ImportStructure>();

            structures.Add(new ImportStructure() { name = "Local Goverment navigation List (LGNL)", file = "lgnl.xml" });
            structures.Add(new ImportStructure() { name = "Scottish Navigation List (SNL)", file = "snl.xml" });

            return structures;
        }

        [HttpGet]
        public bool ImportExampleContent()
        {

        }

        [HttpGet]
        public bool ImportStructure(string file)
        {
            return true;
        }

    }

    public class ImportStructure
    {
        public string name { get; set; }
        public string file { get; set; }
    }
}
