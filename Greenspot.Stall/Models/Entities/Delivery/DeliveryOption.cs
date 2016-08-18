using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greenspot.Stall.Models
{
    public partial class DeliveryOption
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public bool IsTimeDivisible { get; set; }
        public int OptionDivideMinutes { get; set; }
        public string PickUpAddress { get; set; }
        public bool IsPickUp { get; set; }
        public string[] Areas { get; set; }
        public decimal? Fee { get; set; }
    }
}