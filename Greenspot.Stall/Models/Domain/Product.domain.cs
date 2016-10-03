using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Threading.Tasks;
using Greenspot.SDK.Vend;
using System.Data.Entity.Migrations;

namespace Greenspot.Stall.Models
{
    public partial class Product
    {
        #region properties
        public decimal PriceIncTax
        {
            get
            {
                return (Price ?? 0.0M) + (Tax ?? 0.0M);
            }
        }

        [NotMapped]
        public string VariantDescription
        {
            get
            {
                string ds = "";
                if (!string.IsNullOrEmpty(VariantOptionOneValue))
                {
                    ds = VariantOptionOneValue;
                }
                if (!string.IsNullOrEmpty(VariantOptionTwoValue))
                {
                    if (!string.IsNullOrEmpty(ds))
                    {
                        ds += " ";
                    }
                    ds += VariantOptionTwoValue;
                }
                if (!string.IsNullOrEmpty(VariantOptionThreeValue))
                {
                    if (!string.IsNullOrEmpty(ds))
                    {
                        ds += " ";
                    }
                    ds += VariantOptionThreeValue;
                }

                return ds;
            }
        }
        #endregion

        public OperationResult<bool> Save(StallEntities db)
        {
            db.Set<Product>().AddOrUpdate(this);
            return new OperationResult<bool>(db.SaveChanges() > 0);
        }

        public OperationResult<bool> Delete(StallEntities db)
        {
            db.Products.Remove(this);
            return new OperationResult<bool>(db.SaveChanges() > 0);
        }

        #region Static
        //public static IList<Product> GetHomepageProducts(StallEntities db)
        //{
        //    Func<Product, bool> condition = delegate (Product p) { return string.IsNullOrEmpty(p.VariantParentId) && p.Active == true; };
        //    return GetProducts(condition, db).Take(50).ToList();
        //}

        public static IList<Product> Search(StallEntities db, string category, string area, string keyworkd, int page = 0, int pageSize = 10)
        {
            Func<Product, bool> condition = delegate (Product p)
            {
                return (p.Active == true && string.IsNullOrEmpty(p.VariantParentId)
                            && (string.IsNullOrEmpty(category) || p.Stall.StallType.Equals(category))
                            && (string.IsNullOrEmpty(area) || p.Stall.Area.StartsWith(area))
                            && (string.IsNullOrEmpty(keyworkd) || p.BaseName.ToLower().Contains(keyworkd.ToLower())));
            };
            return GetProducts(condition, db).Skip(pageSize * page).Take(pageSize).ToList();
        }

        public static Product FindById(string id, StallEntities db)
        {
            return db.Products.FirstOrDefault(x => x.Id.Equals(id));
        }

        public static bool SetInventoryById(string id, int count, StallEntities db)
        {
            var p = FindById(id, db);
            if (p == null) { return false; }
            p.Stock = count;
            return db.SaveChanges() > 0;
        }

        public static bool DeleteById(string id, StallEntities db)
        {
            var p = FindById(id, db);
            if (p == null) { return false; }
            db.Products.Remove(p);
            return db.SaveChanges() > 0;
        }

        private static IEnumerable<Product> GetProducts(Func<Product, bool> condition, StallEntities db)
        {
            return db.Products.Include(x => x.Stall).Where(x => Stall.StallStatus.Online.Equals(x.Stall.Status)).Where(condition);
        }

        public static Product ConvertFrom(VendProduct p, int stallId)
        {
            return new Product()
            {
                Id = p.Id,
                StallId = stallId,
                SourceId = p.SourceId,
                SourceVariantId = p.VariantSourceId,
                Handle = p.Handle,
                Type = p.Type,
                HasVariants = p.HasVariants,
                VariantParentId = string.IsNullOrEmpty(p.VariantParentId) ? null : p.VariantParentId,
                VariantOptionOneName = p.VariantOptionOneName,
                VariantOptionOneValue = p.VariantOptionOneValue,
                VariantOptionTwoName = p.VariantOptionTwoName,
                VariantOptionTwoValue = p.VariantOptionTwoValue,
                VariantOptionThreeName = p.VariantOptionThreeName,
                VariantOptionThreeValue = p.VariantOptionThreeValue,
                Active = p.Active,
                Name = p.Name,
                BaseName = p.BaseName,
                Description = p.Description,
                Image = p.Image,
                ImageLarge = p.ImageLarge,
                Sku = p.Sku,
                Tags = p.Tags,
                BrandId = p.BrandId,
                BrandName = p.BrandName,
                SupplierName = p.SupplierName,
                SupplierCode = p.SupplierCode,
                SupplierPrice = p.SupplyPrice,
                AccountCodePurchase = p.AccountCodePurchase,
                AccountCodeSales = p.AccountCodeSales,
                TrackInventory = p.TrackInventory,
                Stock = (p.TrackInventory && p.Inventory.Count > 0) ? Convert.ToInt32(p.Inventory[0].Count) : 0,
                Price = p.Price,
                Tax = p.Tax,
                TaxId = p.TaxId,
                TaxRate = p.TaxRate,
                TaxName = p.TaxName,
                DisplayRetailPriceTaxInclusive = p.DisplayRetailPriceTaxInclusive,
                UpdatedAt = p.UpdatedAt,
                DeletedAt = p.DeletedAt
            };
        }
        #endregion
    }
}