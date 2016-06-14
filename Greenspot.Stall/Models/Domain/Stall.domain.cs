using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Data.Entity.Migrations;
using Greenspot.SDK.Vend;
namespace Greenspot.Stall.Models
{
    public partial class Stall
    {
        public async Task<OperationResult<bool>> Init()
        {
            var result = new OperationResult(false);
            //load stall
            var outletResult = await LoadInfo();
            if (!outletResult.Succeeded)
            {
                return outletResult;
            }

            //load product
            var productResult = await LoadProduct();
            if (!productResult.Succeeded)
            {
                return productResult;
            }

            result.Succeeded = Save();
            return result;
        }

        private async Task<OperationResult<bool>> LoadInfo()
        {
            var result = new OperationResult(false);
            //load stall
            var outletResult = await VendOutlet.GetOutletsAsync(Prefix, await StallApplication.GetAccessTokenAsync(Prefix));
            if (outletResult?.Outlets == null || outletResult.Outlets.Count == 0)
            {
                result.Message = "无法同步VEND商铺信息";
                return result;
            }

            //set stall info
            var outlet = outletResult.Outlets[0];
            VendId = outlet.Id;
            Email = outlet.Email;
            PhysicalAddress1 = outlet.PhysicalAddress1;
            PhysicalAddress2 = outlet.PhysicalAddress2;
            PhysicalCity = outlet.PhysicalCity;
            PhysicalCountryId = outlet.PhysicalCountryId;
            PhysicalPostcode = outlet.PhysicalPostcode;
            PhysicalState = outlet.PhysicalState;
            PhysicalSuburb = outlet.PhysicalSuburb;
            RetailerId = outlet.RetailerId;
            TimeZone = outlet.TimeZone;

            //set contact info
            Contacts.Clear();
            Contacts.Add(new StallContact()
            {
                Id = Guid.NewGuid().ToString(),
                CompanyName = outlet.Contact.CompanyName,
                Email = outlet.Contact.Email,
                Fax = outlet.Contact.Fax,
                FirstName = outlet.Contact.FirstName,
                LastName = outlet.Contact.LastName,
                Mobile = outlet.Contact.Mobile,
                Phone = outlet.Contact.Phone,
                //physical address
                PhysicalAddress1 = outlet.Contact.PhysicalAddress1,
                PhysicalAddress2 = outlet.Contact.PhysicalAddress2,
                PhysicalCity = outlet.Contact.PhysicalCity,
                PhysicalCountryId = outlet.Contact.PhysicalCountryId,
                PhysicalPostcode = outlet.Contact.PhysicalPostcode,
                PhysicalState = outlet.Contact.PhysicalState,
                PhysicalSuburb = outlet.Contact.PhysicalSuburb,
                //postal address
                PostalAddress1 = outlet.Contact.PostalAddress1,
                PostalAddress2 = outlet.Contact.PostalAddress2,
                PostalCity = outlet.Contact.PostalCity,
                PostalCountryId = outlet.Contact.PostalCountryId,
                PostalPostcode = outlet.Contact.PostalPostcode,
                PostalState = outlet.Contact.PostalState,
                PostalSuburb = outlet.Contact.PostalSuburb
            });

            result.Succeeded = true;
            return result;
        }

        private async Task<OperationResult<bool>> LoadProduct()
        {
            var result = new OperationResult(false);

            //load stall
            var productResult = await VendProduct.GetProductsAsync(Prefix, await StallApplication.GetAccessTokenAsync(Prefix));
            if (productResult?.Products == null)
            {
                result.Message = "无法同步VEND商品信息";
                return result;
            }

            //set products
            Products.Clear();
            //var products = productResult?.Products.OrderBy(x => x.VariantParentId).ToList();
            var products = productResult?.Products;
            foreach (var p in products)
            {
                Products.Add(new Product()
                {
                    Id = p.Id,
                    StallId = Id,
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
                    Price = p.Price,
                    Tax = p.Tax,
                    TaxId = p.TaxId,
                    TaxRate = p.TaxRate,
                    TaxName = p.TaxName,
                    DisplayRetailPriceTaxInclusive = p.DisplayRetailPriceTaxInclusive,
                    UpdatedAt = p.UpdatedAt,
                    DeletedAt = p.DeletedAt
                });
            }

            //set data
            result.Succeeded = true;
            return result;
        }

        public bool Save()
        {
            using (var db = new StallEntities())
            {
                db.Set<Stall>().AddOrUpdate(this);
                db.Set<Product>().AddOrUpdate(Products.ToArray());
                db.SaveChanges();
                return true;
            }
        }
    }
}