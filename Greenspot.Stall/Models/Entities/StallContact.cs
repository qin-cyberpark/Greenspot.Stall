namespace Greenspot.Stall.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("stall_contacts")]
    public partial class StallContact
    {
        [StringLength(50)]
        public string Id { get; set; }

        [StringLength(50)]
        public string StallId { get; set; }

        [StringLength(100)]
        public string CompanyName { get; set; }

        [StringLength(100)]
        public string Email { get; set; }

        [StringLength(20)]
        public string Fax { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }

        [StringLength(50)]
        public string LastName { get; set; }

        [StringLength(20)]
        public string Mobile { get; set; }

        [StringLength(20)]
        public string Phone { get; set; }

        [StringLength(100)]
        public string PhysicalAddress1 { get; set; }

        [StringLength(100)]
        public string PhysicalAddress2 { get; set; }

        [StringLength(100)]
        public string PhysicalCity { get; set; }

        [StringLength(50)]
        public string PhysicalCountryId { get; set; }

        [StringLength(20)]
        public string PhysicalPostcode { get; set; }

        [StringLength(50)]
        public string PhysicalState { get; set; }

        [StringLength(50)]
        public string PhysicalSuburb { get; set; }

        [StringLength(100)]
        public string PostalAddress1 { get; set; }

        [StringLength(100)]
        public string PostalAddress2 { get; set; }

        [StringLength(100)]
        public string PostalCity { get; set; }

        [StringLength(50)]
        public string PostalCountryId { get; set; }

        [StringLength(20)]
        public string PostalPostcode { get; set; }

        [StringLength(50)]
        public string PostalState { get; set; }

        [StringLength(50)]
        public string PostalSuburb { get; set; }

        [StringLength(50)]
        public string Twitter { get; set; }

        [StringLength(100)]
        public string Website { get; set; }

        [ForeignKey("StallId")]
        public virtual Stall Stall { get; set; }
    }
}
