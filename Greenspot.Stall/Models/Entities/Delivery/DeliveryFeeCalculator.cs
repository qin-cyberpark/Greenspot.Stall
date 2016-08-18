using Newtonsoft.Json;
using System.Collections.Generic;

namespace Greenspot.Stall.Models
{
    public abstract class DeliveryFeeCalculator
    {
        //[JsonProperty("Type")]
        //public string Type { get; set; }

        public DeliveryFeeCalculator()//string type)
        {
            //Type = type;
        }

        //public abstract decimal? Calculate(string depCountryCode, string depCity, string depSuburb,
        //                    string destCountryCode, string destCity, string destSuburb);
        public abstract decimal? Calculate(int? distanceInMeters);
    }
}
