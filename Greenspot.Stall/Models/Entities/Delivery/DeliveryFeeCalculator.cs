using Greenspot.Stall.Models.Settings;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Greenspot.Stall.Models
{
    public interface IDeliveryFeeRule
    {
        decimal? Calculate(DateTime dateTime, string area, int? distanceInMeters, decimal? orderAmount);
    }

    public class DeliveryFeeCalculator
    {
        [JsonProperty("Rules")]
        public List<IDeliveryFeeRule> Rules { get; set; } = new List<IDeliveryFeeRule>();

        public decimal? Calculate(DateTime dateTime, string area, int? distanceInMeters = null, decimal? orderAmount = null)
        {
            foreach (var r in Rules)
            {
                var fee = r.Calculate(dateTime, area, distanceInMeters, orderAmount);
                if (fee != null)
                {
                    return fee;
                }
            }
            return null;
        }
    }
}
