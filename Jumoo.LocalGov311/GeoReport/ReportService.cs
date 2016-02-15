using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Umbraco.Core.Persistence;
using Umbraco.Core.Persistence.DatabaseAnnotations;

namespace Jumoo.LocalGov311.GeoReport
{

    public class GeoServiceList : List<GeoService> { }

    // this will be stored in the db.
    [TableName("GeoServices")]
    [PrimaryKey("Id", autoIncrement = true)]
    public class GeoService
    {
        [PrimaryKeyColumn(AutoIncrement = true)]
        public int id { get; set; }

        public string jurisdiction_id { get; set; }
        public string service_code { get; set; }
        public string service_name { get; set; }
        public string description { get; set; }
        public bool metadata { get; set; }

        public ServiceType type { get; set; }

        public string keywords { get; set; }
        public string group { get; set; }

        public List<ServiceAttribute> attributes { get; set; }
    }

    // this is part of the service (if it's more complex)
    [TableName("ServiceAttributes")]
    public class ServiceAttribute
    {
        [ForeignKey(typeof(GeoService), Name = "FK_ServiceAttribute_GeoService")]
        [IndexAttribute(IndexTypes.NonClustered, Name ="IX_ServiceId")]
        public int ServiceId { get; set; }

        public string variable { get; set; }
        public string code { get; set; }
        public string datatype { get; set; }
        public bool required { get; set; }
        public string datatype_description { get; set; }
        public int order { get; set; }
        public string description { get; set; }
        public Dictionary<string, string> Values { get; set; }
    }

    public enum ServiceType
    {
        realtime, batch, blackbox
    }

    // a request is posted 
    public class ServiceReport
    {
        public string jurisdiction_id { get; set;  }
        public string service_code { get; set; }

        public double lat { get; set; }
        public double lng { get; set; }
        public string address_string { get; set; }
        public int address_id { get; set; }

        public string email { get; set; }
        public string device_id { get; set; }
        public string account_id { get; set; }
        public string first_name { get; set; }
        public string last_name { get; set; }

        public string phone { get; set; }
        public string description { get; set; }
        public string media_url { get; set; }

        public Dictionary<string,string> attibutes { get; set; }
    }

    public class ServiceReponse
    {
        public string service_request_id { get; set; }
        public string token { get; set; }
        public string service_notice { get; set; }
        public string account_id { get; set; }
    }

    public class TokenResponse
    {
        public string service_request_id { get; set; }
        public string token { get; set; }
    }

    public class ServiceRequestList : List<ServiceRequest> { }

    // service request as stored somewhere? 
    //
    // you might not want to store them, because they will
    // get fired through to other services, so you don't
    // want to then keep them local if they are going
    // to get out of sync 

    //
    // but then storing them, will let you operate a 
    // better cache - for giving information out.
    // 
    [TableName("ServiceRequests")]
    public class ServiceRequest
    {
        public string service_request_id { get; set; }
        public ServiceStatus status { get; set; }
        public string status_notes { get; set; }
        public string service_name { get; set; }
        public string service_code { get; set; }
        public string description { get; set; }
        public string agency_responsible { get; set; }
        public string service_notice { get; set; }

        public DateTime requested_datetime { get; set; }
        public DateTime updated_datetime { get; set; }
        public DateTime expected_datetime { get; set; }

        public string address {get;set;}
        public string address_id { get; set; }
        public string zipcode { get; set; }
        public string lat { get; set; }
        public string lng { get; set; }
        public string media_url { get; set; }
    }

    public enum ServiceStatus
    {
        open, closed 
    }

    public class ServiceError
    {
        public string code { get; set; }
        public string description { get; set; }
    }

}
