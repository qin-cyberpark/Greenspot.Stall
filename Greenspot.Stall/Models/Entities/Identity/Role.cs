using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Greenspot.Stall.Models
{
    [Table("greenspot_roles")]
    public partial class Role
    {
        public Role()
        {
            Users = new HashSet<User>();
        }

        [StringLength(50)]
        public string Id { get; set; }
        public string Name { get; set; }

        public virtual ICollection<User> Users { get; set; }
    }
}