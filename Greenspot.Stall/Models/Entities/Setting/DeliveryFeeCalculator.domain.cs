using Newtonsoft.Json;
using System.Collections.Generic;
using System;

namespace Greenspot.Stall.Models
{
    public partial class FixedFeeCalculator
    {
        public override decimal? Calculate(DateTime dateTime, string countryId, string city, string suburb, int? distanceInMeters)
        {
            return Fee;
        }
    }

    public partial class ByAreaCalculator
    {
        public override decimal? Calculate(DateTime dateTime, string countryId, string city, string suburb, int? distanceInMeters)
        {
            if (AreaFees == null)
            {
                return null;
            }

            foreach (var a in AreaFees)
            {
                if (a.Equals(suburb))
                {
                    return a.Fee;
                }
            }

            return null;
        }
    }

    public partial class ByDistanceRangeCalculator
    {
        public override decimal? Calculate(DateTime dateTime, string countryId, string city, string suburb, int? distanceInMeters)
        {
            if (distanceInMeters == null)
            {
                return null;
            }

            int km = distanceInMeters.Value / 1000;
            foreach (var r in Ranges)
            {
                if (r.From <= km && r.To > km)
                {
                    return r.Fee;
                }
            }
            return null;
        }
    }
}
