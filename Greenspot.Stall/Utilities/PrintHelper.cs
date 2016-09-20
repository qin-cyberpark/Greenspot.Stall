using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Greenspot.SDK.MemoBird;
using Greenspot.Configuration;
using Greenspot.Stall.Models;

namespace Greenspot.Stall.Utilities
{
    public class PrintHelper
    {
        private static MemoBirdClient _client = new MemoBirdClient(GreenspotConfiguration.AppSettings["PrinterAccessKey"].Value);
        private object _locker = new object();
        public static bool PrintOrder(Order order)
        {
            //print order
            var rep = _client.Print(order.Stall.PrinterAddress, 48952, new Receipt(order).Image);
            if (rep != null && rep.Succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}