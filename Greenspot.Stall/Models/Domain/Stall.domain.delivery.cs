using Greenspot.SDK.Vend;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Greenspot.Stall.Models
{
    public partial class Stall
    {
        public IList<DeliveryOrPickupOption> GetPickupOptions(DateTime dtStart, int nextDays)
        {
            if (nextDays < 0 || nextDays > Setting.MaxAdvancedOrderDays)
            {
                //stall advanced order days
                nextDays = Setting.MaxAdvancedOrderDays;
            }

            var options = Setting.Pickup.GetOptions(dtStart, nextDays, Setting.OpeningHours);
            var result = new List<DeliveryOrPickupOption>();
            foreach (var opt in options)
            {
                var newOpt = DeliveryOrPickupOption.Parse(opt);
                //delivery fee is 0
                newOpt.Fee = 0;
                result.Add(newOpt);
            }

            return result.Where(x => x.ReferenceTimePonit > dtStart).OrderBy(x => x.PickUpAddress).ThenBy(x => x.From).ToList();
        }

        public IList<DeliveryOrPickupOption> GetDeliveryOptions(DateTime dtStart, int nextDays,
                                                string area, int? distanceInMeters = null, decimal? orderAmount = null)
        {
            if (nextDays < 0 || nextDays > Setting.MaxAdvancedOrderDays)
            {
                //stall advanced order days
                nextDays = Setting.MaxAdvancedOrderDays;
            }

            var options = Setting.Delivery.GetOptions(dtStart, nextDays, Setting.OpeningHours);
            var result = new List<DeliveryOrPickupOption>();
            foreach (var opt in options)
            {
                if (!Models.Area.IsApplicable(opt.Areas, area))
                {
                    continue;
                }

                var newOpt = DeliveryOrPickupOption.Parse(opt);
                newOpt.IsStoreDelivery = true;
                newOpt.Fee = opt.Calculator.Calculate(opt.From, area, distanceInMeters, orderAmount);
                if (newOpt.Fee != null)
                {
                    result.Add(newOpt);
                }
            }

            return result.Where(x => x.ReferenceTimePonit > dtStart).OrderBy(x => x.From).ToList();
        }
    }
}