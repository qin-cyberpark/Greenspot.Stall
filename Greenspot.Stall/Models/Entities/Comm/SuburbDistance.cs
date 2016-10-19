
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace Greenspot.Stall.Models
{
    [Table("comm_suburb_distances")]
    public partial class SuburbDistance
    {
        [StringLength(200)]
        public string ID { get; set; }

        [StringLength(3)]
        public string DepartureCountryCode { get; set; }

        [StringLength(50)]
        public string DepartureStateOrRegion { get; set; }

        [StringLength(100)]
        public string DepartureCity { get; set; }

        [StringLength(100)]
        public string DepartureSuburb { get; set; }

        [StringLength(20)]
        public string DeparturePostcode { get; set; }

        [StringLength(3)]
        public string DestinationCountryCode { get; set; }

        [StringLength(50)]
        public string DestinationStateOrRegion { get; set; }

        [StringLength(100)]
        public string DestinationCity { get; set; }

        [StringLength(100)]
        public string DestinationSuburb { get; set; }

        [StringLength(20)]
        public string DestinationPostcode { get; set; }

        public int? Meters { get; set; }
    }
}
