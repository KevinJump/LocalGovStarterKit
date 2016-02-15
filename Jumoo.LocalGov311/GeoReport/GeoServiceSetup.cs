using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Umbraco.Core;
using Umbraco.Core.Logging;
using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.SqlSyntax;

namespace Jumoo.LocalGov311.GeoReport
{
    public class GeoServiceSetup : ApplicationEventHandler
    {
        protected override void ApplicationStarted(UmbracoApplicationBase umbracoApplication, ApplicationContext applicationContext)
        {
            var db = applicationContext.DatabaseContext.Database;

            var creator = new DatabaseSchemaHelper(db, LoggerResolver.Current.Logger, SqlSyntaxContext.SqlSyntaxProvider);

            if (!creator.TableExist("GeoServices"))
                creator.CreateTable<GeoService>(false);

            if (!creator.TableExist("ServiceAttributes"))
                creator.CreateTable<ServiceAttribute>(false);
            
        }
    }
}
