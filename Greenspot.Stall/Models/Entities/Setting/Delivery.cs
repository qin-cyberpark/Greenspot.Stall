using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Greenspot.Stall.Models.Settings
{
    public partial class DeliveryDefinition
    {
        [JsonProperty("MinOrderAmount")]
        public decimal? MinOrderAmount { get; set; }

        [JsonProperty("FreeDeliveryOrderAmount")]
        public decimal? FreeDeliveryOrderAmount { get; set; }

        [JsonProperty("DefaultCalculators")]
        public IList<DeliveryFeeCalculator> DefaultCalculators { get; set; }

        [JsonProperty("Rules")]
        public IList<DeliveryRule> Rules { get; set; }
    }

    public partial class DeliveryRule
    {
        [JsonProperty("Hours")]
        public DateTimeTerm Hours { get; set; }

        [JsonProperty("Areas")]
        public IList<string> Areas { get; set; } = new List<string>();

        [JsonProperty("Calculators")]
        public IList<DeliveryFeeCalculator> Calculators { get; set; }
    }

    public partial class DeliveryOption : DateTimePair
    {
        public IList<string> Areas { get; set; }
        public IList<DeliveryFeeCalculator> Calculators { get; set; }

        public override T Create<T>(DateTime from, DateTime to)
        {
            var obj = base.Create<T>(from, to) as DeliveryOption;
            if (obj != null)
            {
                obj.Areas = Areas;
                obj.Calculators = Calculators;
            }
            return obj as T;
        }
    }
}