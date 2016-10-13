using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greenspot.SDK.Vend
{
    public partial class Supplier
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("retailer_id")]
        public string RetailerId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("contact")]
        public VendContact Contact { get; set; }
    }


    public class SupplierApiResult
    {
        [JsonProperty("suppliers")]
        public IList<Supplier> Suppliers { get; set; }
    }
}
