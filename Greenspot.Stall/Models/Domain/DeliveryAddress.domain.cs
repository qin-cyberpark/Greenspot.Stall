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

        public static DeliveryAddress FindById(int id, StallEntities db)
        {
            return db.DeliveryAddresses.FirstOrDefault(x => x.Id == id);
        }

        public bool Save(StallEntities db)
        {
            db.DeliveryAddresses.Add(this);
            return db.SaveChanges() >= 0;
        }

        public override string ToString()
        {
            return string.Format("{0} {1}\n{2}", Name, Mobile, FullAddress);
        }
    }
}