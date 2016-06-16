using System;
using System.Collections.Generic;
using System.Linq;
using System.Data.Entity;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Greenspot.Stall.Models
{
    public partial class Product
    {
        #region properties
        [NotMapped]
        public decimal TotalPrice
        {
            get
            {
                return (Price??0.0M) + (Tax??0.0M);
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

        public static IList<Product> GetHomepageProducts(StallEntities db)
        {
            Func<Product, bool> condition = delegate (Product p) { return string.IsNullOrEmpty(p.VariantParentId); };
            return GetProducts(condition, db).Take(10).ToList();
        }

        public static IList<Product> Search(string keyworkd, StallEntities db, int takeAmount = 50)
        {
            Func<Product, bool> condition = delegate (Product p)
            {
                return string.IsNullOrEmpty(p.VariantParentId) && p.BaseName.ToLower().Contains(keyworkd.ToLower());
            };
            return GetProducts(condition, db).Take(takeAmount).ToList();
        }

        public static Product FindById(string id, StallEntities db)
        {
            return db.Products.Include(x=>x.Stall).FirstOrDefault(x => x.Id.Equals(id));
        }

        private static IEnumerable<Product> GetProducts(Func<Product, bool> condition, StallEntities db)
        {
            return db.Products.Where(condition).Where(x => !x.Handle.Equals("vend-discount"));
        }

    }
}