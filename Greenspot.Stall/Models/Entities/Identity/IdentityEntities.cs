namespace Greenspot.Stall.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class IdentityEntities : DbContext
    {
        public IdentityEntities()
            : base("name=StallEntities")
        {
        }

        public virtual DbSet<UserSnsInfo> UserSnsInfos { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
