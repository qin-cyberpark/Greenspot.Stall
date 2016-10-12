namespace Greenspot.Stall.Models
{
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    [Table("products")]
    public partial class Product
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Product()
        {
            Variants = new HashSet<Product>();
        }

        [StringLength(50)]
        public string Id { get; set; }

        public int StallId { get; set; }

        [StringLength(100)]
        public string SourceId { get; set; }

        [StringLength(100)]
        public string SourceVariantId { get; set; }

        [StringLength(100)]
        public string Handle { get; set; }

        [StringLength(50)]
        public string Type { get; set; }

        [Column(TypeName = "bit")]
        public bool? HasVariants { get; set; }

        [StringLength(50)]
        public string VariantParentId { get; set; }

        [StringLength(50)]
        public string VariantOptionOneName { get; set; }

        [StringLength(50)]
        public string VariantOptionOneValue { get; set; }

        [StringLength(50)]
        public string VariantOptionTwoName { get; set; }

        [StringLength(50)]
        public string VariantOptionTwoValue { get; set; }

        [StringLength(50)]
        public string VariantOptionThreeName { get; set; }

        [StringLength(50)]
        public string VariantOptionThreeValue { get; set; }

        [Column(TypeName = "bit")]
        public bool Active { get; set; }

        [StringLength(100)]
        public string Name { get; set; }

        [StringLength(100)]
        public string BaseName { get; set; }

        [Column(TypeName = "text")]
        [StringLength(65535)]
        public string Description { get; set; }

        [StringLength(150)]
        public string Image { get; set; }

        [StringLength(150)]
        public string ImageLarge { get; set; }

        [StringLength(50)]
        public string Sku { get; set; }

        [StringLength(100)]
        public string Tags { get; set; }

        [StringLength(50)]
        public string BrandId { get; set; }

        [StringLength(100)]
        public string BrandName { get; set; }

        [StringLength(100)]
        public string SupplierName { get; set; }

        [StringLength(50)]
        public string SupplierCode { get; set; }

        public decimal? SupplierPrice { get; set; }

        [StringLength(50)]
        public string AccountCodePurchase { get; set; }

        [StringLength(50)]
        public string AccountCodeSales { get; set; }

        [Column(TypeName = "bit")]
        public bool TrackInventory { get; set; }

        public int Stock { get; set; }

        public decimal? Price { get; set; }

        public decimal? Tax { get; set; }

        [StringLength(50)]
        public string TaxId { get; set; }

        public decimal? TaxRate { get; set; }

        [StringLength(50)]
        public string TaxName { get; set; }

        public decimal? DisplayRetailPriceTaxInclusive { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public DateTime? DeletedAt { get; set; }

        public int RecommendIndex { get; set; }

        [NotMapped]
        public string LineNote { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<Product> Variants { get; set; }

        [ForeignKey("VariantParentId")]
        public virtual Product ParentProduct { get; set; }

        [ForeignKey("StallId")]
        public virtual Stall Stall { get; set; }

    }
}
