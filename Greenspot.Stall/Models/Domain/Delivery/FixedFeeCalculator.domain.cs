using Newtonsoft.Json;
using System.Collections.Generic;

namespace Greenspot.Stall.Models
{
    public partial class FixedFeeCalculator : DeliveryFeeCalculator
    {
        public override decimal? Calculate(int? distanceInMeters)
        {
            return Fee;
        }
    }
}
