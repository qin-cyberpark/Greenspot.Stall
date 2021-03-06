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

        //public virtual DbSet<StallContact> Contacts { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Stall> Stalls { get; set; }
        public virtual DbSet<User> Users { get; set; }
        public virtual DbSet<Role> Roles { get; set; }
        public virtual DbSet<VendAccessToken> VendAccessTokens { get; set; }
        //public virtual DbSet<VendWebhook> VendWebhooks { get; set; }
        public virtual DbSet<Order> Orders { get; set; }
        public virtual DbSet<OrderItem> OrderItems { get; set; }
        public virtual DbSet<Area> Areas { get; set; }
        public virtual DbSet<Suburb> Suburbs { get; set; }
        public virtual DbSet<SuburbDistance> SuburbDistances { get; set; }
        public virtual DbSet<DeliveryAddress> DeliveryAddresses { get; set; }
        public virtual DbSet<Payment> Payments { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>()
                .HasMany<Role>(u => u.Roles)
                .WithMany(r => r.Users)
                .Map(ur =>
                {
                    ur.MapLeftKey("UserId");
                    ur.MapRightKey("RoleId");
                    ur.ToTable("greenspot_user_roles");
                });

        }
    }
}
