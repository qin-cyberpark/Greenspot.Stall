using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Web;

namespace Greenspot.Stall.Models
{
    public partial class Payment
    {
        public bool Save(StallEntities db)
        {
            db.Payments.AddOrUpdate(this);
            return db.SaveChanges() > 0;
        }
    }
}