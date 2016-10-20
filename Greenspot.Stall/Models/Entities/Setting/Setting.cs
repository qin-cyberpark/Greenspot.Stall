using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using Greenspot.Stall.Models.Settings;

namespace Greenspot.Stall.Models
{
    public partial class Setting
    {
        [JsonProperty("MaxAdvancedOrderDays")]
        public int MaxAdvancedOrderDays { get; set; } = 3;

        [JsonProperty("MinPickupAdvancedMinutes")]
        public int MinPickupAdvancedMinutes { get; set; } = 60;

        [JsonProperty("MinDeliveryAdvancedMinutes")]
        public int MinDeliveryAdvancedMinutes { get; set; } = 90;

        [JsonProperty("OpeningHours")]
        public DateTimeTerm OpeningHours { get; set; }

        [JsonProperty("Pickup")]
        public PickupDefinition Pickup { get; set; }

        [JsonProperty("Delivery")]
        public DeliveryDefinition Delivery { get; set; }
    }
}