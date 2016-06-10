namespace Greenspot.Stall.Models
{
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;

    public partial class StallEntities : DbContext
    {
        public StallEntities()
            : base("name=StallEntities")
        {
        }

        public virtual DbSet<Contact> Contacts { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Stall> Stalls { get; set; }
        public virtual DbSet<VendAccessToken> VendAccessTokens { get; set; }
        public virtual DbSet<VendWebhook> VendWebhooks { get; set; }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
        }
    }
}
