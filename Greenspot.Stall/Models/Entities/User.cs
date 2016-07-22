using Greenspot.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greenspot.Stall.Models
{
    [Table("users")]
    public partial class User
    {
        public User()
        {
            Stalls = new HashSet<Stall>();
            DeliveryAddresses = new HashSet<DeliveryAddress>();
            CreditCards = new HashSet<CreditCard>();
        }

        [Key]
        [StringLength(50)]
        public string Id { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        public virtual ICollection<Stall> Stalls { get; set; }
        public virtual ICollection<DeliveryAddress> DeliveryAddresses { get; set; }
        public virtual ICollection<CreditCard> CreditCards { get; set; }
    }
}
