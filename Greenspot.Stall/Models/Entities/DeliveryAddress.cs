namespace Greenspot.Stall.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("customer_delivery_addresses")]
    public partial class DeliveryAddress
    {
        [Key, Column(Order = 0)]
        [StringLength(50)]
        public string UserId { get; set; }

        [Key, Column(Order = 1)]
        [StringLength(50)]
        public string Code { get; set; } = Guid.NewGuid().ToString();

        [StringLength(50)]
        public string Name { get; set; }

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

        [StringLength(100)]
        public string State { get; set; }


        [StringLength(100)]
        public string Area { get; set; }

        [StringLength(50)]
        public string Suburb { get; set; }

        [ForeignKey("UserId")]
        [JsonIgnore]
        public virtual User User { get; set; }
    }
}