namespace Greenspot.Stall.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("stalls")]
    public partial class Stall
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Stall()
        {
            Contacts = new HashSet<StallContact>();
            Products = new HashSet<Product>();
            Registers = new HashSet<Register>();
            Webhooks = new HashSet<VendWebhook>();
        }

        [StringLength(50)]
        public string Id { get; set; }

        [Required]
        [StringLength(50)]
        public string UserId { get; set; }

        [StringLength(50)]
        public string Prefix { get; set; }

        [StringLength(50)]
        public string VendId { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(50)]
        public string PaymentTypeId { get; set; }

        [StringLength(100)]
        public string PhysicalAddress1 { get; set; }

        [StringLength(100)]
        public string PhysicalAddress2 { get; set; }

        [StringLength(100)]
        public string PhysicalCity { get; set; }

        [StringLength(50)]
        public string PhysicalCountryId { get; set; }

        [StringLength(20)]
        public string PhysicalPostcode { get; set; }

        [StringLength(50)]
        public string PhysicalState { get; set; }

        [StringLength(50)]
        public string PhysicalSuburb { get; set; }

        [StringLength(50)]
        public string RetailerId { get; set; }

        [StringLength(100)]
        public string TimeZone { get; set; }

        [Column(TypeName = "bit")]
        public bool? Approved { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Products { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<VendWebhook> Webhooks { get; set; }

        public virtual ICollection<StallContact> Contacts { get; set; }
        public virtual ICollection<Register> Registers { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
