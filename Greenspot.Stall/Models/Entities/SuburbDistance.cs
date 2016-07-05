using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greenspot.Stall.Models
{
    [Table("suburb_distances")]
    public class SuburbDistance
    {
        [StringLength(50)]
        public string ID { get; set; }

        [StringLength(50)]
        public string CountryCode { get; set; }

        [StringLength(100)]
        public string City { get; set; }

        [StringLength(100)]
        public string OriginSubRegion { get; set; }

        [StringLength(100)]
        public string OriginSuburb { get; set; }


        [StringLength(100)]
        public string DestinationSubRegion { get; set; }

        [StringLength(100)]
        public string DestinationSuburb { get; set; }

        public int? Meters { get; set; }
    }
}
