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

        public static DeliveryAddress FindByCode(string userId, string code, StallEntities db)
        {
            return db.DeliveryAddresses.FirstOrDefault(x => x.UserId.Equals(userId) && x.Code.Equals(code));
        }

        public bool Save(StallEntities db)
        {
            db.DeliveryAddresses.Add(this);
            return db.SaveChanges() >= 0;
        }

        public override string ToString()
        {
            return string.Format("{0}, {1}, {2}", Name, Mobile, FullAddress);
        }

        public string FullAddress
        {
            get
            {
                return (string.IsNullOrEmpty(Address2) ? "" : Address2 + ", ") +
                    string.Format("{0}, {1}, {2} {3}", Address1, Suburb, City, Postcode);
            }
        }
    }
}