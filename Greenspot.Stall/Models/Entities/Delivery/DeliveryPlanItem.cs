using Newtonsoft.Json;
using System.Collections.Generic;

namespace Greenspot.Stall.Models
{
    public partial class DeliveryPlanItem
    {
        [JsonProperty("Periods")]
        public IList<DeliveryPlanPeriod> Periods { get; set; }

        [JsonProperty("PickUpAddress")]
        public string PickUpAddress { get; set; }

        [JsonProperty("FilterByArea")]
        public bool FilterByArea { get; set; } = true;

        [JsonProperty("Areas")]
        public string[] Areas { get; set; }

        [JsonProperty("IsExclusive")]
        public bool IsExclusive { get; set; } = true;

        [JsonProperty("ExclusiveExtensionMinutes")]
        public ExclusiveExtensionMinutes ExclusiveExtension { get; set; }

        [JsonProperty("FeeCalculators")]
        public IList<DeliveryFeeCalculator> Calculators { get; set; }

        public class ExclusiveExtensionMinutes
        {
            [JsonProperty("Before")]
            public int Before { get; set; } = 120;

            [JsonProperty("After")]
            public int After { get; set; } = 120;
        }
    }
}
