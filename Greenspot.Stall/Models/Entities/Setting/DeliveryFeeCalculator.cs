using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Greenspot.Stall.Models
{
    public abstract class DeliveryFeeCalculator
    {

        public DeliveryFeeCalculator()
        {

        }

        public abstract decimal? Calculate(DateTime dateTime, string countryCode, string city, string suburb, int? distanceInMeters);
    }

    public partial class FixedFeeCalculator : DeliveryFeeCalculator
    {
        [JsonProperty("Fee")]
        public decimal? Fee { get; set; }
    }


    public partial class ByAreaCalculator : DeliveryFeeCalculator
    {
        [JsonProperty("Definition")]
        public IList<AreaFee> AreaFees { get; set; }

        public class AreaFee
        {
            [JsonProperty("Area")]
            public string Area { get; set; }

            [JsonProperty("Fee")]
            public decimal Fee { get; set; }
        }
    }

    public partial class ByDistanceRangeCalculator : DeliveryFeeCalculator
    {
        [JsonProperty("Definition")]
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
