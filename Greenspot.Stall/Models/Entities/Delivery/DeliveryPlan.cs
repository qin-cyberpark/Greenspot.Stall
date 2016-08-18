﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Greenspot.Stall.Models
{
    public partial class DeliveryPlan
    {
        [JsonProperty("DefaultFee")]
        public decimal? DefaultFee { get; set; }

        [JsonProperty("FreeDeliveryOrderAmount")]
        public int? FreeDeliveryOrderAmount { get; set; }

        [JsonProperty("MaxAdvancedOrderDays")]
        public int MaxAdvancedOrderDays { get; set; } = 7;

        [JsonProperty("OptionDivideMinutes")]
        public int OptionDivideMinutes { get; set; } = 120;

        [JsonProperty("DefaultPlans")]
        public IList<DeliveryPlanItem> DefaultPlans { get; set; }

        [JsonProperty("TemporaryPlans")]
        public IList<DeliveryPlanItem> TemporaryPlans { get; set; }
    }
}