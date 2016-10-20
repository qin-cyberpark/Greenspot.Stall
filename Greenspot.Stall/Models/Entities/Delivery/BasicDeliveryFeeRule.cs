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
        public IList<string> Areas { get; set; } = new List<string>();

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

        public decimal? Calculate(DateTime dateTime, string area, int? distanceInMeters, decimal? orderAmount)
        {
            //date time rule has not been implemented

            //Area
            if (Areas != null && Areas.Count > 0)
            {
                if (string.IsNullOrEmpty(area))
                {
                    return null;
                }

                bool matched = false;
                foreach (var a in Areas)
                {
                    if (area.StartsWith(a))
                    {
                        matched = true;
                        break;
                    }
                }

                if (!matched)
                {
                    return null;
                }
            }

            //distance
            if ((DistanceFrom != null || DistanceTo != null) && distanceInMeters == null)
            {
                return null;
            }

            if (DistanceFrom != null && distanceInMeters.Value < DistanceFrom.Value)
            {
                return null;
            }

            if (DistanceTo != null && distanceInMeters.Value > DistanceTo.Value)
            {
                return null;
            }

            //order amount
            if ((OrderAmountFrom != null || OrderAmountTo != null) && orderAmount == null)
            {
                return null;
            }

            if (OrderAmountFrom != null && orderAmount.Value < OrderAmountFrom.Value)
            {
                return null;
            }

            if (OrderAmountTo != null && orderAmount.Value > OrderAmountTo.Value)
            {
                return null;
            }

            return Fee;
        }
    }
}
