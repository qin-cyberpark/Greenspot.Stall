using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace Greenspot.Stall.Models
{
    [Table("registers")]
    public partial class Register
    {
        public string Id { get; set; }

        [StringLength(50)]
        public string Name { get; set; }

        [StringLength(50)]
        public string StallId { get; set; }

        [ForeignKey("StallId")]
        public virtual Stall Stall {get;set;}
    }
}