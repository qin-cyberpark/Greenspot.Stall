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
        public Stall()
        {
            //Contacts = new HashSet<StallContact>();
            Products = new HashSet<Product>();
            Webhooks = new HashSet<VendWebhook>();
        }

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(100)]
        public string StallName { get; set; }

        [Required]
        [StringLength(50)]
        public string UserId { get; set; }

        [StringLength(50)]
        public string Prefix { get; set; }
        
        [StringLength(50)]
        public string RetailerId { get; set; }

        [StringLength(50)]
        public string OutletId { get; set; }

        [StringLength(50)]
        public string ContactName { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(20)]
        public string Mobile { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(100)]
        public string Address1 { get; set; }

        [StringLength(100)]
        public string Address2 { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        [StringLength(50)]
        public string CountryId { get; set; }

        [StringLength(20)]
        public string Postcode { get; set; }

        [StringLength(50)]
        public string State { get; set; }

        [StringLength(50)]
        public string Suburb { get; set; }

        [StringLength(50)]
        public string RegisterName { get; set; }

        [StringLength(50)]
        public string RegisterId { get; set; }

        [StringLength(100)]
        public string TimeZone { get; set; }


        [StringLength(50)]
        public string DeliveryProductId { get; set; }

        [StringLength(50)]
        public string PaymentTypeId { get; set; }

        [StringLength(250)]
        public string QrCodeImage { get; set; }

        public decimal? MinOrderAmount { get; set; }

        public int? DefaultOrderAdvancedMinutes { get; set; }


        [StringLength(50)]
        public string BankAccount { get; set; }

        public decimal? Balance { get; set; }

        [Column(TypeName = "text")]
        public string DeliveryPlanJsonString { get; set; }

        [Column(TypeName = "bit")]
        public bool? Approved { get; set; }

        public virtual ICollection<Product> Products { get; set; }

        public virtual ICollection<VendWebhook> Webhooks { get; set; }

        //public virtual ICollection<StallContact> Contacts { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}
