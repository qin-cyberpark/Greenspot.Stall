using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greenspot.Stall.Models
{
    public partial class DeliveryAddress
    {
        public static IList<DeliveryAddress> FindByUserId(string id, StallEntities db)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            return db.DeliveryAddresses.Where(x => x.UserId.Equals(id)).ToList();
        }
    }
}