namespace Greenspot.Stall.Models
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("vend_access_tokens")]
    public partial class VendAccessToken
    {
        [Key]
        [StringLength(100)]
        public string Prefix { get; set; }

        [Required]
        [StringLength(50)]
        public string AccessToken { get; set; }

        [Required]
        [StringLength(50)]
        public string TokenType { get; set; }

        public long Expires { get; set; }

        public int ExpiresIn { get; set; }

        [Required]
        [StringLength(50)]
        public string RefreshToken { get; set; }
    }
}
