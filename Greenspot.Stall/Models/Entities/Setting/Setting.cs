using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Greenspot.Stall.Models.Settings;

namespace Greenspot.Stall.Models
{
    public partial class Setting
    {
        [JsonProperty("MaxAdvancedOrderDays")]
        public int MaxAdvancedOrderDays { get; set; }

        [JsonProperty("MinOrderAdvancedMinutes")]
        public decimal MinOrderAdvancedMinutes { get; set; }

        [JsonProperty("OpeningHours")]
        public DateTimeTerm OpeningHours { get; set; }

        [JsonProperty("Pickup")]
        public PickupDefinition Pickup { get; set; }

        [JsonProperty("Delivery")]
        public DeliveryDefinition Delivery { get; set; }
    }
}