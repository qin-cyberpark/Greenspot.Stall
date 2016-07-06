using Newtonsoft.Json;
using System.Collections.Generic;

namespace Greenspot.Stall.Models
{
    public partial class DeliveryFee
    {
        public class Types
        {
            private Types()
            {

            }
            public const string ByRange = "ByRange";
        }

        [JsonProperty("Default")]
        public decimal Default { get; set; }

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
            public decimal Fee { get; set; }
        }
        #endregion
    }
}