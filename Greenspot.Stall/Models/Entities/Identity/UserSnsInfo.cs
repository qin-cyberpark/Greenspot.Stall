using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greenspot.Stall.Models
{
    [Table("greenspot_user_snsinfos")]
    public partial class UserSnsInfo
    {
        [Key, Column(Order = 0)]
        [StringLength(128)]
        public string UserId { get; set; }

        [Key, Column(Order = 1)]
        [StringLength(128)]
        public string SnsName { get; set; }

        [Key, Column(Order = 2)]
        [StringLength(256)]
        public string InfoKey { get; set; }

        public string InfoValue { get; set; }

        [ForeignKey("UserId")]
        public virtual User User { get; set; }
    }
}