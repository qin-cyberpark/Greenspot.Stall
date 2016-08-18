using Newtonsoft.Json;
using System.Collections.Generic;

namespace Greenspot.Stall.Models
{
    public partial class ByRangeCalculator : DeliveryFeeCalculator
    {
        //public ByRangeCalculator() : base("ByRange")
        //{

        //}

        [JsonProperty("Ranges")]
        public IList<DistanceRange> Ranges { get; set; }

        public class DistanceRange
        {
            [JsonProperty("From")]
            public int From { get; set; }

            [JsonProperty("To")]
            public int To { get; set; }

            [JsonProperty("Fee")]
            public decimal Fee { get; set; }
        }
    }
}
