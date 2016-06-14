using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greenspot.SDK.Vend
{
    public partial class VendOutlet
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("retailer_id")]
        public string RetailerId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("time_zone")]
        public string TimeZone { get; set; }

        [JsonProperty("tax_id")]
        public string TaxId { get; set; }

        [JsonProperty("contact")]
        public VendContact Contact { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("physical_address1")]
        public string PhysicalAddress1 { get; set; }

        [JsonProperty("physical_address2")]
        public string PhysicalAddress2 { get; set; }

        [JsonProperty("physical_suburb")]
        public string PhysicalSuburb { get; set; }

        [JsonProperty("physical_city")]
        public string PhysicalCity { get; set; }

        [JsonProperty("physical_postcode")]
        public string PhysicalPostcode { get; set; }

        [JsonProperty("physical_state")]
        public string PhysicalState { get; set; }

        [JsonProperty("physical_country_id")]
        public string PhysicalCountryId { get; set; }
    }

    public class VendOutletApiResult
    {
        [JsonProperty("outlets")]
        public IList<VendOutlet> Outlets { get; set; }
    }
}