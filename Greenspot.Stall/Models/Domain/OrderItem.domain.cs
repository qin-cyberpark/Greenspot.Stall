using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greenspot.Stall.Models
{
    public partial class OrderItem
    {
        public OrderItem(Product product)
        {
            StallId = product.StallId;
            ProductId = product.Id;
            SourceId = product.SourceId;
            SourceVariantId = product.SourceVariantId;
            Handle = product.Handle;
            Type = product.Type;
            HasVariants = product.HasVariants;
            VariantOptionOneName = product.VariantOptionOneName;
            VariantOptionOneValue = product.VariantOptionOneValue;
            VariantOptionTwoName = product.VariantOptionTwoName;
            VariantOptionTwoValue = product.VariantOptionTwoValue;
            VariantOptionThreeName = product.VariantOptionThreeName;
            VariantOptionThreeValue = product.VariantOptionThreeValue;
            Active = product.Active;
            Name = product.Name;
            BaseName = product.BaseName;
            Description = product.Description;
            Image = product.Image;
            ImageLarge = product.ImageLarge;
            Sku = product.Sku;
            Tags = product.Tags;
            BrandId = product.BrandId;
            BrandName = product.BrandName;
            SupplierName = product.SupplierName;
            SupplierCode = product.SupplierCode;
            SupplierPrice = product.SupplierPrice;
            AccountCodePurchase = product.AccountCodePurchase;
            AccountCodeSales = product.AccountCodeSales;
            TrackInventory = product.TrackInventory;
            Price = product.Price;
            Tax = product.Tax;
            TaxId = product.TaxId;
            TaxRate = product.TaxRate;
            TaxName = product.TaxName;
            DisplayRetailPriceTaxInclusive = product.DisplayRetailPriceTaxInclusive;
            LineNote = product.LineNote;
        }

        public decimal PriceIncTax
        {
            get
            {
                return (Price ?? 0.0M) + (Tax ?? 0.0M);
            }
        }
    }
}
