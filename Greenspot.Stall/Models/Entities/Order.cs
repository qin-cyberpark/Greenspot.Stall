using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

namespace Greenspot.Stall.Models
{
    [Table("orders")]
    public partial class Order
    {
        public class OrderStatus
        {
            private OrderStatus()
            {

            }

            public const string OPEN = "OPEN";
            public const string SAVED = "SAVED";
            public const string CLOSED = "CLOSED";
            public const string LAYBY = "LAYBY";
            public const string LAYBY_CLOSED = "LAYBY_CLOSED";
            public const string ONACCOUNT = "ONACCOUNT";
            public const string ONACCOUNT_CLOSED = "ONACCOUNT_CLOSED";
            public const string VOIDED = "VOIDED";
        }

        public Order()
        {
            Items = new HashSet<OrderItem>();
        }

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [StringLength(50)]
        public string VendSaleId { get; set; }

        public int StallId { get; set; }

        [StringLength(50)]
        public string UserId { get; set; }

        public decimal TotalCharge { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;
        public DateTime? PaidTime { get; set; }

        [StringLength(50)]
        public string Status { get; set; }

        [JsonIgnore]
        [NotMapped]
        public CreditCard SelectedCard { get; set; } = null;

        public string Note { get; set; }

        //[JsonIgnore]
        //public string JsonString { get; set; }

        [JsonIgnore]
        public string VendResponse { get; set; }

        [JsonIgnore]
        public string PxPayResponse { get; set; }

        [ForeignKey("StallId")]
        [JsonIgnore]
        public virtual Stall Stall { get; set; }

        public virtual ICollection<OrderItem> Items { get; set; }
    }
}
