﻿using System;
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

        [StringLength(50)]
        public string Id { get; set; }

        [StringLength(50)]
        public string VendSaleId { get; set; }

        [StringLength(50)]
        public string StallId { get; set; }

        [StringLength(50)]
        public string UserId { get; set; }

        public DateTime CreateTime { get; set; } = DateTime.Now;

        public string Status { get; set; }

        public DateTime? PaidTime { get; set; }

        [JsonIgnore]
        public string JsonString { get; set; }

        [JsonIgnore]
        public string VendResponse { get; set; }

        [ForeignKey("StallId")]
        [JsonIgnore]
        public virtual Stall Stall { get; set; }

        [NotMapped]
        public IList<OrderItem> Items { get; set; } = new List<OrderItem>();
    }

    public class OrderItem
    {
        public Product Product { get; set; }
        public int Quantity { get; set; }
    }
}
