using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Greenspot.SDK.MemoBird;
using Greenspot.Configuration;
using Greenspot.Stall.Models;
using System.Threading.Tasks;

namespace Greenspot.Stall.Utilities
{
    public class PrintHelper
    {
        private static MemoBirdClient _client = new MemoBirdClient(GreenspotConfiguration.AppSettings["PrinterAccessKey"].Value);
        private static object _locker = new object();
        public static async Task<bool> PrintOrderAsync(Order order)
        {
            //lock (_locker)
            //{
            //print order
            var rep = await _client.PrintAsync(order.Stall.PrinterAddress, 48952, new Receipt(order).Image);
            if (rep != null && rep.Succeeded)
            {
                return true;
            }
            else
            {
                return false;
            }
            //}
        }
    }
}