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
    [Table("payments")]
    public partial class Payment
    {
        public Payment()
        {
            Orders = new HashSet<Order>();
        }

        [DatabaseGeneratedAttribute(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public decimal Amount { get; set; }

        public DateTime? CreateTime { get; set; } = DateTime.Now;
        public DateTime? ResponseTime { get; set; }
        public bool HasPaid { get; set; }
        public string OrderIds { get; set; }

        [JsonIgnore]
        public string PxPayResponse { get; set; }

        public virtual ICollection<Order> Orders { get; set; }
    }
}
