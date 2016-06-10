using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greenspot.SDK.Vend
{
    public class VendContact
    {

        [JsonProperty("first_name")]
        public object FirstName { get; set; }

        [JsonProperty("last_name")]
        public object LastName { get; set; }

        [JsonProperty("company_name")]
        public string CompanyName { get; set; }

        [JsonProperty("phone")]
        public object Phone { get; set; }

        [JsonProperty("mobile")]
        public object Mobile { get; set; }

        [JsonProperty("fax")]
        public object Fax { get; set; }

        [JsonProperty("email")]
        public object Email { get; set; }

        [JsonProperty("twitter")]
        public object Twitter { get; set; }

        [JsonProperty("website")]
        public object Website { get; set; }

        [JsonProperty("physical_address1")]
        public object PhysicalAddress1 { get; set; }

        [JsonProperty("physical_address2")]
        public object PhysicalAddress2 { get; set; }

        [JsonProperty("physical_suburb")]
        public object PhysicalSuburb { get; set; }

        [JsonProperty("physical_city")]
        public object PhysicalCity { get; set; }

        [JsonProperty("physical_postcode")]
        public object PhysicalPostcode { get; set; }

        [JsonProperty("physical_state")]
        public object PhysicalState { get; set; }

        [JsonProperty("physical_country_id")]
        public object PhysicalCountryId { get; set; }

        [JsonProperty("postal_address1")]
        public object PostalAddress1 { get; set; }

        [JsonProperty("postal_address2")]
        public object PostalAddress2 { get; set; }

        [JsonProperty("postal_suburb")]
        public object PostalSuburb { get; set; }

        [JsonProperty("postal_city")]
        public object PostalCity { get; set; }

        [JsonProperty("postal_postcode")]
        public object PostalPostcode { get; set; }

        [JsonProperty("postal_state")]
        public object PostalState { get; set; }

        [JsonProperty("postal_country_id")]
        public object PostalCountryId { get; set; }
    }

}