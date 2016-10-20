using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Greenspot.Stall.Models.Settings;

namespace Greenspot.Stall.Models
{
    public partial class DeliveryOrPickupOption
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public TimeDivisionTypes DivisionType { get; set; }
        public int DivisionMinutes { get; set; }
        public bool IsPickUp { get; set; }
        public IList<string> PickUpAddresses { get; set; }
        public bool IsStoreDelivery { get; set; }
        public IList<string> Areas { get; set; }
        public decimal? Fee { get; set; }
    }
}