using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greenspot.Stall.Models
{
    public partial class Stall
    {
        public static IList<Stall> FindByUserId(string userId, StallEntities db)
        {
            if (string.IsNullOrEmpty(userId))
            {
                return null;
            }

            return db.Stalls.Where(x => x.UserId.Equals(userId)).ToList();
        }
    }
}