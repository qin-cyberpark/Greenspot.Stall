using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Greenspot.Stall.Models.Settings
{
    public enum DeliveryTypes : int { Unavailable = 0, StoreOnly, PlatformOnly, PlatformAsComplement, LowerFee }

    public partial class DeliveryDefinition
    {
        [JsonProperty("DeliveryType")]
        public DeliveryTypes DeliveryType { get; set; }

        [JsonProperty("MinOrderAmount")]
        public decimal? MinOrderAmount { get; set; }

        [JsonProperty("FreeDeliveryOrderAmount")]
        public decimal? FreeDeliveryOrderAmount { get; set; }

        [JsonProperty("DefaultCalculator")]
        public DeliveryFeeCalculator DefaultCalculator { get; set; }

        [JsonProperty("Rules")]
        public IList<DeliveryRule> Rules { get; set; }
    }

    public partial class DeliveryRule
    {
        [JsonProperty("DateTimes")]
        public DateTimeTerm DateTimes { get; set; }

        [JsonProperty("SameAsOpeningHours")]
        public bool SameAsOpeningHours { get; set; }

        [JsonProperty("Areas")]
        public IList<string> Areas { get; set; } = new List<string>();

        [JsonProperty("Calculator")]
        public DeliveryFeeCalculator Calculator { get; set; }
    }

    public partial class DeliveryOption : DateTimePair
    {
        public IList<string> Areas { get; set; }
        public DeliveryFeeCalculator Calculator { get; set; }

        public override T Create<T>(DateTime from, DateTime to)
        {
            var obj = base.Create<T>(from, to) as DeliveryOption;
            if (obj != null)
            {
                obj.Areas = Areas;
                obj.Calculator = Calculator;
            }
            return obj as T;
        }
    }
}