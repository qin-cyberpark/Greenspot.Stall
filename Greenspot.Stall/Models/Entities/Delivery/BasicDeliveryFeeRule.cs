using Newtonsoft.Json;
using System.Collections.Generic;
using System;
using Greenspot.Stall.Models.Settings;
namespace Greenspot.Stall.Models
{
    public class BasicDeliveryFeeRule : IDeliveryFeeRule
    {
        //area
        [JsonProperty("Areas")]
        public IList<string> Areas { get; set; }

        //hours
        [JsonProperty("DateTimes")]
        public DateTimeTerm DateTimes { get; set; }

        //distance
        [JsonProperty("DistanceFrom")]
        public int? DistanceFrom { get; set; }
        [JsonProperty("DistanceTo")]
        public int? DistanceTo { get; set; }

        //order amount
        [JsonProperty("OrderAmountFrom")]
        public decimal? OrderAmountFrom { get; set; }
        [JsonProperty("OrderAmountTo")]
        public decimal? OrderAmountTo { get; set; }

        //Fee
        [JsonProperty("Fee")]
        public decimal Fee { get; set; }

        public decimal? Calculate(DateTime dateTime, string countryCode, string city, string suburb, int? distanceInMeters, decimal? orderAmount)
        {
            return 99;
        }
    }
}
