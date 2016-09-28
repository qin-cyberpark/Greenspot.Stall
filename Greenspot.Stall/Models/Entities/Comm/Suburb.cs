using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greenspot.Stall.Models
{
    [Table("comm_suburbs")]
    public partial class Suburb
    {
        [StringLength(50)]
        public string ID { get; set; }

        [StringLength(50)]
        public string  CountryCode { get; set; }

        [StringLength(50)]
        public string State { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        [StringLength(100)]
        public string Area { get; set; }

        [StringLength(100)]
        public string Name { get; set; }
    }
}
