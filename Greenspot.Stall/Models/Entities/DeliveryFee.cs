using Newtonsoft.Json;
using System.Collections.Generic;

namespace Greenspot.Stall.Models
{
    public class DeliveryFee
    {

        [JsonProperty("Default")]
        public double Default { get; set; }

        [JsonProperty("FreeDeliveryOrderAmount")]
        public int FreeDeliveryOrderAmount { get; set; }

        [JsonProperty("Type")]
        public string Type { get; set; }

        #region By Range
        [JsonProperty("Ranges")]
        public IList<FeeRange> Ranges { get; set; }

        public class FeeRange
        {
            [JsonProperty("From")]
            public int From { get; set; }

            [JsonProperty("To")]
            public int To { get; set; }

            [JsonProperty("Fee")]
            public double Fee { get; set; }
        }
        #endregion
    }
}