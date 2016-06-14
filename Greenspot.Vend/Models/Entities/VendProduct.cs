using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections;

namespace Greenspot.SDK.Vend
{
    public partial class VendImageLinks
    {

        [JsonProperty("original")]
        public string Original { get; set; }

        [JsonProperty("standard")]
        public string Standard { get; set; }

        [JsonProperty("thumb")]
        public string Thumb { get; set; }
    }

    public class VendImage
    {

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("links")]
        public VendImageLinks Links { get; set; }
    }

    public class VendInventory
    {

        [JsonProperty("outlet_id")]
        public string OutletId { get; set; }

        [JsonProperty("outlet_name")]
        public string OutletName { get; set; }

        [JsonProperty("count")]
        public string Count { get; set; }

        [JsonProperty("reorder_point")]
        public string ReorderPoint { get; set; }

        [JsonProperty("restock_level")]
        public string RestockLevel { get; set; }
    }

    public class VendPriceBookEntry
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("product_id")]
        public string ProductId { get; set; }

        [JsonProperty("price_book_id")]
        public string PriceBookId { get; set; }

        [JsonProperty("price_book_name")]
        public string PriceBookName { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("outlet_name")]
        public string OutletName { get; set; }

        [JsonProperty("outlet_id")]
        public string OutletId { get; set; }

        [JsonProperty("customer_group_name")]
        public string CustomerGroupName { get; set; }

        [JsonProperty("customer_group_id")]
        public string CustomerGroupId { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }

        [JsonProperty("loyalty_value")]
        public object LoyaltyValue { get; set; }

        [JsonProperty("tax")]
        public double Tax { get; set; }

        [JsonProperty("tax_id")]
        public string TaxId { get; set; }

        [JsonProperty("tax_rate")]
        public double TaxRate { get; set; }

        [JsonProperty("tax_name")]
        public string TaxName { get; set; }

        [JsonProperty("display_retail_price_tax_inclusive")]
        public int DisplayRetailPriceTaxInclusive { get; set; }

        [JsonProperty("min_units")]
        public string MinUnits { get; set; }

        [JsonProperty("max_units")]
        public string MaxUnits { get; set; }

        [JsonProperty("valid_from")]
        public string ValidFrom { get; set; }

        [JsonProperty("valid_to")]
        public string ValidTo { get; set; }
    }

    public class VendTax
    {

        [JsonProperty("outlet_id")]
        public string OutletId { get; set; }

        [JsonProperty("tax_id")]
        public string TaxId { get; set; }
    }

    public partial class VendProduct
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("source_id")]
        public string SourceId { get; set; }

        [JsonProperty("variant_source_id")]
        public string VariantSourceId { get; set; }

        [JsonProperty("handle")]
        public string Handle { get; set; }

        [JsonProperty("type")]
        public string Type { get; set; }

        [JsonProperty("has_variants")]
        public bool HasVariants { get; set; }

        [JsonProperty("variant_parent_id")]
        public string VariantParentId { get; set; }

        [JsonProperty("variant_option_one_name")]
        public string VariantOptionOneName { get; set; }

        [JsonProperty("variant_option_one_value")]
        public string VariantOptionOneValue { get; set; }

        [JsonProperty("variant_option_two_name")]
        public string VariantOptionTwoName { get; set; }

        [JsonProperty("variant_option_two_value")]
        public string VariantOptionTwoValue { get; set; }

        [JsonProperty("variant_option_three_name")]
        public string VariantOptionThreeName { get; set; }

        [JsonProperty("variant_option_three_value")]
        public string VariantOptionThreeValue { get; set; }

        [JsonProperty("active")]
        public bool Active { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("base_name")]
        public string BaseName { get; set; }

        [JsonProperty("description")]
        public string Description { get; set; }

        [JsonProperty("image")]
        public string Image { get; set; }

        [JsonProperty("image_large")]
        public string ImageLarge { get; set; }

        [JsonProperty("images")]
        public IList<VendImage> Images { get; set; }

        [JsonProperty("sku")]
        public string Sku { get; set; }

        [JsonProperty("tags")]
        public string Tags { get; set; }

        [JsonProperty("brand_id")]
        public string BrandId { get; set; }

        [JsonProperty("brand_name")]
        public string BrandName { get; set; }

        [JsonProperty("supplier_name")]
        public string SupplierName { get; set; }

        [JsonProperty("supplier_code")]
        public string SupplierCode { get; set; }

        [JsonProperty("supply_price")]
        public decimal SupplyPrice { get; set; }

        [JsonProperty("account_code_purchase")]
        public string AccountCodePurchase { get; set; }

        [JsonProperty("account_code_sales")]
        public string AccountCodeSales { get; set; }

        [JsonProperty("track_inventory")]
        public bool TrackInventory { get; set; }

        [JsonProperty("inventory")]
        public IList<VendInventory> Inventory { get; set; }

        [JsonProperty("price_book_entries")]
        public IList<VendPriceBookEntry> PriceBookEntries { get; set; }

        [JsonProperty("price")]
        public decimal? Price { get; set; }

        [JsonProperty("tax")]
        public decimal? Tax { get; set; }

        [JsonProperty("tax_id")]
        public string TaxId { get; set; }

        [JsonProperty("tax_rate")]
        public decimal? TaxRate { get; set; }

        [JsonProperty("tax_name")]
        public string TaxName { get; set; }

        [JsonProperty("taxes")]
        public IList<VendTax> Taxes { get; set; }

        [JsonProperty("display_retail_price_tax_inclusive")]
        public int DisplayRetailPriceTaxInclusive { get; set; }

        [JsonProperty("updated_at")]
        public DateTime? UpdatedAt { get; set; }

        [JsonProperty("deleted_at")]
        public DateTime? DeletedAt { get; set; }
    }

    public class VendProductApiResult 
    {
        [JsonProperty("products")]
        public IList<VendProduct> Products { get; set; }
    }
}
