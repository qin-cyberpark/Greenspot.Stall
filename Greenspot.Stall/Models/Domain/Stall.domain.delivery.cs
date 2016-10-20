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
            if (nextDays <= 0 || nextDays > Setting.MaxAdvancedOrderDays)
            {
                //stall advanced order days
                nextDays = Setting.MaxAdvancedOrderDays;
            }

            var options = Setting.Pickup.GetOptions(dtStart, nextDays, Setting.OpeningHours);
            var result = new List<DeliveryOrPickupOption>();
            foreach (var opt in options)
            {
                result.Add(DeliveryOrPickupOption.Parse(opt));
            }

            return result.OrderBy(x => x.From).ToList();
        }
    }
}