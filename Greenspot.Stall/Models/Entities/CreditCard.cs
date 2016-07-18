using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Greenspot.Stall.Models
{
    [Table("customer_cards")]
    public partial class CreditCard
    {
        public int Id { get; set; }

        [StringLength(50)]
        public string UserId { get; set; }

        [StringLength(16)]
        public string DspBillingId { get; set; }

        [StringLength(50)]
        public string CardNumber { get; set; }

        [StringLength(20)]
        public string CardName { get; set; }

        [StringLength(4)]
        public string DateExpiry { get; set; }

        [ForeignKey("UserId")]
        [JsonIgnore]
        public virtual User User { get; set; }
    }
}