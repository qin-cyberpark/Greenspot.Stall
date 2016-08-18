using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greenspot.Stall.Models
{
    public partial class ByRangeCalculator
    {
        public override decimal? Calculate(int? distanceInMeters)
        {
            if(distanceInMeters == null)
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