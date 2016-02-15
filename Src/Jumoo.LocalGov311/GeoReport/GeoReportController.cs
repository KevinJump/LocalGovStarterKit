using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Http;
using Umbraco.Web.WebApi;

using Umbraco.Core.Persistence;
using Umbraco.Core;

namespace Jumoo.LocalGov311.GeoReport
{
    public class GeoReportController : UmbracoApiController
    {
        /// <summary>
        ///  gets all the services...
        /// </summary>
        new public GeoServiceList Services()
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;

            var sql = new Sql().Select("*").From();

            var s = db.Query<GeoService>(sql);

            var serviceList = new GeoServiceList();
            serviceList.AddRange(s);

            return serviceList;
        }

        /// <summary>
        ///  gets a service definition
        /// </summary>
        new public GeoServiceList Services(string code)
        {
            var db = ApplicationContext.Current.DatabaseContext.Database;

            var sql = new Sql().Where<GeoService>(x => x.service_code == code);
            var s = db.Query<GeoService>(sql);

            var serviceList = new GeoServiceList();
            serviceList.AddRange(s);

            return serviceList;
        }

        [HttpPost]
        public ServiceReponse Requests(ServiceReport request)
        {
            return new ServiceReponse
            {
                service_notice = "your request has been received"
            };
        }

        public TokenResponse Tokens(string token)
        {
            return new TokenResponse
            {
                token = token,
                service_request_id = "1"
            };
        }

        [HttpGet]
        public ServiceRequestList Requests(int requestId, DateTime? start, DateTime? end)
        {
            return new ServiceRequestList();
        }

        public ServiceRequestList Requests(int id)
        {
            return new ServiceRequestList();
        }


    }
}
