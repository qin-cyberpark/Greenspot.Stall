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
    [Table("greenspot_users")]
    public partial class User
    {
        public User()
        {
            Stalls = new HashSet<Stall>();
            DeliveryAddresses = new HashSet<DeliveryAddress>();
            CreditCards = new HashSet<CreditCard>();
            Roles = new HashSet<Role>();
            SnsInfos = new HashSet<UserSnsInfo>();
        }

        [Key]
        [StringLength(50)]
        public string Id { get; set; }

        public virtual ICollection<Stall> Stalls { get; set; }
        public virtual ICollection<DeliveryAddress> DeliveryAddresses { get; set; }
        public virtual ICollection<CreditCard> CreditCards { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<UserSnsInfo> SnsInfos { get; set; }
    }
}
