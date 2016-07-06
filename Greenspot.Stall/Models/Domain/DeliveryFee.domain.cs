using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greenspot.Stall.Models
{
    public partial class DeliveryFee
    {
        #region Operation
        internal decimal? Get(string oriCountryId, string oriCity, string oriSuburb, 
                            string destCountryId, string destCity, string destSuburb, decimal orderAmount = 0)
        {
            var meters = GetDistance(oriCountryId, oriCity, oriSuburb, destCountryId, destCity, destSuburb);
            if(meters == null)
            {
                return null;
            }

            if(orderAmount >= FreeDeliveryOrderAmount)
            {
                return 0;
            }

            switch (Type)
            {
                case Types.ByRange:
                    return GetByRange(meters.Value);
                default:
                    return null;
            }
        }

        private int? GetDistance(string oriCountryId, string oriCity, string oriSuburb,
                            string destCountryId, string destCity, string destSuburb)
        {
            if (string.IsNullOrEmpty(oriCountryId) || string.IsNullOrEmpty(oriCity) || string.IsNullOrEmpty(oriSuburb)
               || string.IsNullOrEmpty(destCountryId) || string.IsNullOrEmpty(destCity) || string.IsNullOrEmpty(destSuburb))
            {
                return null;
            }

            if (!oriCountryId.Equals(destCountryId) || !oriCity.Equals(destCity))
            {
                return null;
            }

            if (oriSuburb.Equals(destSuburb))
            {
                return 0;
            }

            var suburb1 = oriSuburb;
            var suburb2 = destSuburb;
            if(oriSuburb.CompareTo(destSuburb) > 0)
            {
                suburb1 = destSuburb;
                suburb2 = oriSuburb;
            }

            using (var db = new StallEntities())
            {
                var distance = db.SuburbDistances.FirstOrDefault(x => x.CountryCode.Equals(oriCountryId) && x.City.Equals(oriCity)
                                        && x.OriginSuburb.Equals(suburb1) && x.DestinationSuburb.Equals(suburb2));

                return distance?.Meters;
            }
        }
        private decimal GetByRange(int meters)
        {
            int km = meters / 1000;
            foreach (var r in Ranges)
            {
                if(r.From <= km && r.To > km)
                {
                    return r.Fee;
                }
            }
            return Default;
        }
        #endregion
    }
}
