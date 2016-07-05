using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greenspot.SDK.Vend
{
    public partial class VendWebhookRequest
    {
        public class VendWebhookTypes
        {
            private VendWebhookTypes() { }
            public const string ProductUpdate = "product.update";
            public const string InventoryUpdate = "inventory.update";
        }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }
    }

    public partial class VendWebhook
    {
        public class VendWebhookTypes
        {
            private VendWebhookTypes() { }
            public const string ProductUpdate = "product.update";
            public const string InventoryUpdate = "inventory.update";
        }

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("retailer_id")]
        public string RetailerId { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("url")]
        public string Url { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }
    }
}
