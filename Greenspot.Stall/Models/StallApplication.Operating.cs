using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greenspot.Stall
{
    public partial class StallApplication
    {
        private HashSet<int> _operatingPayments = new HashSet<int>();

        public static bool IsPaymentOperating(int paymentId)
        {
            return _instance._operatingPayments.Contains(paymentId);
        }

        public static void AddOperatingPayment(int paymentId)
        {
            _instance._operatingPayments.Add(paymentId);
        }

        public static void RemoveOperatingPayment(int paymentId)
        {
            _instance._operatingPayments.Remove(paymentId);
        }
    }
}