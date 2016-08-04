namespace Greenspot.Stall.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("vend_webhooks")]
    public partial class VendWebhook
    {

        [StringLength(50)]
        public string Id { get; set; }

        public int StallId { get; set; }

        [StringLength(50)]
        public string Prefix { get; set; }

        [StringLength(50)]
        public string RetailerId { get; set; }

        [StringLength(50)]
        public string Type { get; set; }

        [StringLength(250)]
        public string Url { get; set; }

        [Column(TypeName = "bit")]
        public bool? Active { get; set; }
    }
}
