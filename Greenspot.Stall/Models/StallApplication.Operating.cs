using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greenspot.Stall
{
    public partial class StallApplication
    {
        private HashSet<int> _operatingOrders = new HashSet<int>();

        public static bool IsOrderOperating(int orderId)
        {
            return _instance._operatingOrders.Contains(orderId);
        }

        public static void AddOperatingOrder(int orderId)
        {
            _instance._operatingOrders.Add(orderId);
        }

        public static void RemoveOperatingOrder(int orderId)
        {
            _instance._operatingOrders.Remove(orderId);
        }
    }
}