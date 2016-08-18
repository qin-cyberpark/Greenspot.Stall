using Newtonsoft.Json;
using System.Collections.Generic;

namespace Greenspot.Stall.Models
{
    public partial class FixedFeeCalculator : DeliveryFeeCalculator
    {
        [JsonProperty("Fee")]
        public decimal? Fee { get; set; }
    }
}
