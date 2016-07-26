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
        internal decimal? Get(string depCountryCode, string depCity, string depSuburb,
                            string destCountryCode, string destCity, string destSuburb, decimal orderAmount = 0)
        {
            var meters = GetDistance(depCountryCode, depCity, depSuburb, destCountryCode, destCity, destSuburb);
            if (meters == null)
            {
                return null;
            }

            if (orderAmount >= FreeDeliveryOrderAmount)
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

        private int? GetDistance(string depCountryCode, string depCity, string depSuburb,
                            string destCountryCode, string destCity, string destSuburb)
        {
            if (string.IsNullOrEmpty(depCountryCode) || string.IsNullOrEmpty(depCity) || string.IsNullOrEmpty(depSuburb)
               || string.IsNullOrEmpty(destCountryCode) || string.IsNullOrEmpty(destCity) || string.IsNullOrEmpty(destSuburb))
            {
                return null;
            }

            if (!depCountryCode.Equals(destCountryCode) || !depCity.Equals(destCity))
            {
                return null;
            }

            if (depSuburb.Equals(destSuburb))
            {
                return 0;
            }

            return Utilities.DistanceMatrix.GetSuburbDistaince(depCountryCode, depCity, depSuburb, destCountryCode, destCity, destSuburb);
        }

        private decimal GetByRange(int meters)
        {
            int km = meters / 1000;
            foreach (var r in Ranges)
            {
                if (r.From <= km && r.To > km)
                {
                    return r.Fee;
                }
            }
            return Default;
        }
        #endregion
    }
}
