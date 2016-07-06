using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greenspot.Stall.Models.ViewModels
{
    public class DeliveryTimeViewModel
    {
        public DateTime From;
        public DateTime To;
        public bool IsPickUp;
        public string Area;
        public string PickUpAddress;
    }
}