using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greenspot.Stall.Models
{
    public partial class Payment
    {
        public static Payment CreatePayment(StallEntities db, decimal amount, string orderIds = null)
        {
            //set total price
            var payment = new Payment()
            {
                Amount = amount,
                CreateTime = DateTime.Now,
                OrderIds = orderIds,
                HasPaid = false
            };
            db.Payments.Add(payment);
            if (db.SaveChanges() > 0)
            {
                return payment;
            }
            else
            {
                return null;
            }
        }
    }
}