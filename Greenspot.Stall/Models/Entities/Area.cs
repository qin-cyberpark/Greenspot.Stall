using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greenspot.Stall.Models
{
    [Table("areas")]
    public partial class Area
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
        public string Name { get; set; }
    }
}
