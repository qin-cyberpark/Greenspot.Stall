using Greenspot.SDK.Vend;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace Greenspot.Stall.Models
{
    public partial class Stall
    {
        public static async Task<OperationResult> InitMultiStall(int id, StallEntities db)
        {
            var result = new OperationResult(false);
            var baseStall = Stall.FindById(id, db);
            if (baseStall == null)
            {
                result.Message = $"Stall {id} is not exist";
                return result;
            }

            //load base info
            var infoResult = await baseStall.LoadInfo();
            if (!infoResult.Succeeded)
            {
                result.Message = infoResult.Message;
                return result;
            }

            //load product
            var productResult = await baseStall.LoadProduct();
            if (!productResult.Succeeded)
            {
                result.Message = productResult.Message;
                return result;
            }

            //create webhook
            var webhookResult = await baseStall.CreateWebhook();
            if (!webhookResult.Succeeded)
            {
                result.Message = webhookResult.Message;
                return result;
            }

            //load suppliers
            var supplierResult = await SDK.Vend.Supplier.GetSuppliersAsync(baseStall.Prefix, await StallApplication.GetAccessTokenAsync(baseStall.Prefix));
            var suppliers = supplierResult.Suppliers;
            if (suppliers == null || suppliers.Count == 0)
            {
                //
                result.Message = "无法获取VEND Supplier信息";
                return result;
            }

            //seperate by supplier
            var unStalledProduct = new List<Product>();
            var stalls = new SortedDictionary<string, Stall>();

            foreach (var p in baseStall.Products)
            {
                //loop product
                if (string.IsNullOrEmpty(p.SupplierName))
                {
                    //no supplier product
                    unStalledProduct.Add(p);
                    continue;
                }

                Stall currStall;


                if (stalls.ContainsKey(p.SupplierName))
                {
                    //exist stall
                    currStall = stalls[p.SupplierName];
                }
                else
                {
                    #region new stall
                    var supplier = suppliers.FirstOrDefault(x => x.Name.Equals(p.SupplierName));

                    //new stall
                    currStall = new Stall()
                    {
                        UserId = baseStall.UserId,
                        Prefix = baseStall.Prefix,
                        StallName = p.SupplierName,
                        RetailerId = baseStall.RetailerId,
                        RegisterId = baseStall.RegisterId,
                        RegisterName = baseStall.RegisterName,
                        OutletId = baseStall.OutletId,
                        PaymentTypeId = baseStall.PaymentTypeId,
                        DeliveryProductId = baseStall.DeliveryProductId,
                        DiscountProductId = baseStall.DiscountProductId,
                        ContactName = $"{supplier.Contact.FirstName} {supplier.Contact.LastName}".Trim(),
                        Mobile = supplier.Contact.Mobile,
                        Phone = supplier.Contact.Phone,
                        Address1 = supplier.Contact.PhysicalAddress1,
                        Address2 = supplier.Contact.PhysicalAddress2,
                        City = supplier.Contact.PhysicalCity,
                        CountryId = supplier.Contact.PhysicalCountryId,
                        Postcode = supplier.Contact.PhysicalPostcode,
                        State = supplier.Contact.PhysicalState,
                        Suburb = supplier.Contact.PhysicalSuburb,
                        TimeZone = baseStall.TimeZone,
                        Status = StallStatus.Offline
                    };

                    stalls.Add(p.SupplierName, currStall);
                    #endregion
                }

                currStall.Products.Add(p);
            }

            //save to db
            using (var trans = db.Database.BeginTransaction())
            {
                //save base stall
                try
                {
                    //keep unstalled product
                    baseStall.Products = unStalledProduct;
                    db.Set<Stall>().AddOrUpdate(baseStall);
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    result.Message = $"failed to save stall {baseStall.StallName}:{ex.ToString()}";
                    trans.Rollback();
                    return result;
                }

                foreach (var s in stalls.Values)
                {
                    try
                    {
                        db.Set<Stall>().AddOrUpdate(s);
                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        result.Message = $"failed to save stall {s.StallName}:{ex.ToString()}";
                        trans.Rollback();
                        return result;
                    }
                }

                trans.Commit();
            }

            result.Succeeded = true;
            return result;
        }

        //private static async Task<OperationResult<Stall>> LoadVendInfo(string prefix)
        //{
        //    var result = new OperationResult<Stall>(false);
        //    var stall = new Stall()
        //    {
        //        Prefix = prefix
        //    };

        //    //load stall
        //    var outletResult = await VendOutlet.GetOutletsAsync(prefix, await StallApplication.GetAccessTokenAsync(prefix));
        //    if (outletResult?.Outlets == null || outletResult.Outlets.Count == 0)
        //    {
        //        result.Message = "无法同步VEND商铺信息";
        //        return result;
        //    }

        //    //set stall info
        //    var outlet = outletResult.Outlets[0];
        //    stall.RetailerId = outlet.RetailerId;
        //    stall.OutletId = outlet.Id;
        //    stall.TimeZone = outlet.TimeZone;

        //    //load payment types
        //    var paytypeResult = await VendPaymentType.GetPaymentTypetsAsync(prefix, await StallApplication.GetAccessTokenAsync(prefix));
        //    if (paytypeResult?.PaymentTypes == null || paytypeResult.PaymentTypes.Count == 0)
        //    {
        //        result.Message = "无法获取VEND PAYMENT TYPE信息";
        //        return result;
        //    }

        //    //set payment type
        //    stall.PaymentTypeId = null;
        //    foreach (var pt in paytypeResult.PaymentTypes)
        //    {
        //        if (pt.Name.ToUpper().Equals("JDL PAY"))
        //        {
        //            stall.PaymentTypeId = pt.Id;
        //            break;
        //        }
        //    }

        //    //
        //    if (string.IsNullOrEmpty(stall.PaymentTypeId))
        //    {
        //        result.Message = "无法获取名为JDL PAY的VEND PAYMENT TYPE";
        //        return result;
        //    }

        //    //load registers
        //    var registerResult = await VendRegister.GetRegistersAsync(prefix, await StallApplication.GetAccessTokenAsync(prefix));
        //    if (registerResult?.Registers == null || registerResult.Registers.Count == 0)
        //    {
        //        result.Message = "无法获取VEND REGISTER信息";
        //        return result;
        //    }

        //    //set registers
        //    var reg = registerResult.Registers[0];
        //    stall.RegisterName = reg.Name;
        //    stall.RegisterId = reg.Id;

        //    //load product
        //    var productResult = await VendProduct.GetProductsAsync(prefix, await StallApplication.GetAccessTokenAsync(prefix));
        //    if (productResult?.Products == null)
        //    {
        //        result.Message = "无法获取VEND商品信息";
        //        return result;
        //    }

        //    //var products = productResult?.Products.OrderBy(x => x.VariantParentId).ToList();
        //    var vProducts = productResult?.Products;
        //    foreach (var vP in vProducts)
        //    {
        //        var p = Product.ConvertFrom(vP, 0);
        //        if (p.Handle.ToLower().Equals("vend-discount"))
        //        {
        //            stall.DiscountProductId = p.Id;
        //            p.Active = false;
        //        }
        //        else if (p.Handle.ToLower().Equals("jdl-delivery"))
        //        {
        //            stall.DeliveryProductId = p.Id;
        //            p.Active = false;
        //        }
        //        stall.Products.Add(p);
        //    }


        //    result.Data = stall;
        //    result.Succeeded = true;

        //    return result;
        //}
    }
}