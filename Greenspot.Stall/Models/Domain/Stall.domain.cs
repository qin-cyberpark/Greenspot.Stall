using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Data.Entity.Migrations;
using Greenspot.SDK.Vend;
using System.ComponentModel.DataAnnotations.Schema;
using Greenspot.Configuration;
using Newtonsoft.Json;
using System.Data.Entity;

namespace Greenspot.Stall.Models
{
    public partial class Stall
    {
        public class StallStatus
        {
            public const string Applied = "APPLIED";
            public const string Offline = "OFFLINE";
            public const string Online = "ONLINE";
        }
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


            //create webhook
            var webhookResult = await CreateWebhook();
            if (!webhookResult.Succeeded)
            {
                return webhookResult;
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
            RetailerId = outlet.RetailerId;
            OutletId = outlet.Id;
            //Email = outlet.Email;
            Address1 = outlet.PhysicalAddress1;
            Address2 = outlet.PhysicalAddress2;
            City = outlet.PhysicalCity;
            CountryId = outlet.PhysicalCountryId;
            Postcode = outlet.PhysicalPostcode;
            State = outlet.PhysicalState;
            Suburb = outlet.PhysicalSuburb;
            TimeZone = outlet.TimeZone;

            //load payment types
            var paytypeResult = await VendPaymentType.GetPaymentTypetsAsync(Prefix, await StallApplication.GetAccessTokenAsync(Prefix));
            if (paytypeResult?.PaymentTypes == null || paytypeResult.PaymentTypes.Count == 0)
            {
                result.Message = "无法获取VEND PAYMENT TYPE信息";
                return result;
            }

            //set payment type
            PaymentTypeId = null;
            foreach (var pt in paytypeResult.PaymentTypes)
            {
                if (pt.Name.ToUpper().Equals("JDL PAY"))
                {
                    PaymentTypeId = pt.Id;
                    break;
                }
            }

            if (string.IsNullOrEmpty(PaymentTypeId))
            {
                result.Message = "无法获取名为JDL PAY的VEND PAYMENT TYPE";
                return result;
            }

            //load registers
            var registerResult = await VendRegister.GetRegistersAsync(Prefix, await StallApplication.GetAccessTokenAsync(Prefix));
            if (registerResult?.Registers == null || registerResult.Registers.Count == 0)
            {
                result.Message = "无法获取VEND REGISTER信息";
                return result;
            }

            //set registers
            var reg = registerResult.Registers[0];
            RegisterName = reg.Name;
            RegisterId = reg.Id;

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
                result.Message = "无法获取VEND商品信息";
                return result;
            }

            //set products
            Products.Clear();

            //var products = productResult?.Products.OrderBy(x => x.VariantParentId).ToList();
            var vProducts = productResult?.Products;
            foreach (var vP in vProducts)
            {
                var p = Product.ConvertFrom(vP, Id);
                if (p.Handle.ToLower().Equals("vend-discount"))
                {
                    p.Active = false;
                }
                else if (p.Handle.ToLower().Equals("gs-delivery"))
                {
                    DeliveryProductId = p.Id;
                    p.Active = false;
                }
                Products.Add(p);
            }

            //set data
            result.Succeeded = true;
            return result;
        }

        //public async Task<OperationResult<bool>> CreateDelivery()
        //{
        //    var result = new OperationResult(false);

        //    //create delivery product
        //    var pdtDelivery = new SDK.Vend.VendProduct()
        //    {
        //        Name = "运费",
        //        BaseName = "运费",
        //        Handle = "gs-delivery",
        //        Sku = "gs-delivery",
        //        Price = 0.0M,
        //        Active = true
        //    };

        //    var pdtResp = await SDK.Vend.VendProduct.AddProduct(Prefix, await StallApplication.GetAccessTokenAsync(Prefix), pdtDelivery);
        //    if (pdtResp == null || pdtResp.Product == null || string.IsNullOrEmpty(pdtResp.Product.Id))
        //    {
        //        result.Message = "无法创建运费";
        //        return result;
        //    }

        //    //set data
        //    result.Succeeded = true;
        //    return result;
        //}

        private async Task<OperationResult<bool>> CreateWebhook()
        {
            var result = new OperationResult(false);
            //load webhooks
            var webhooks = await SDK.Vend.VendWebhook.GetWebhooksAsync(Prefix, await StallApplication.GetAccessTokenAsync(Prefix));
            Webhooks.Clear();
            foreach (var h in webhooks)
            {
                Webhooks.Add(new VendWebhook()
                {
                    Id = h.Id,
                    StallId = Id,
                    Prefix = Prefix,
                    RetailerId = h.RetailerId,
                    Type = h.Type,
                    Url = h.Url,
                    Active = h.Active
                });
            }

            if (!webhooks.Any(x => SDK.Vend.VendWebhook.VendWebhookTypes.ProductUpdate.Equals(x.Type)))
            {
                //create product update webhook
                var pdtUpdate = new SDK.Vend.VendWebhook()
                {
                    Type = SDK.Vend.VendWebhook.VendWebhookTypes.ProductUpdate,
                    Url = GreenspotConfiguration.AppSettings["webhook.product.update"].Value,
                    Active = true
                };
                var pdtResp = await SDK.Vend.VendWebhook.CreateVendWebhookAsync(pdtUpdate, Prefix, await StallApplication.GetAccessTokenAsync(Prefix));
                if (pdtResp != null)
                {
                    //save
                    Webhooks.Add(new VendWebhook()
                    {
                        Id = pdtResp.Id,
                        StallId = Id,
                        Prefix = Prefix,
                        RetailerId = pdtResp.RetailerId,
                        Type = pdtResp.Type,
                        Url = pdtResp.Url,
                        Active = pdtResp.Active
                    });
                }
                else
                {
                    result.Message = "无法创建商品更新WEBHOOK";
                    return result;
                }
            }


            if (!webhooks.Any(x => SDK.Vend.VendWebhook.VendWebhookTypes.InventoryUpdate.Equals(x.Type)))
            {
                //create inventory update webhook
                var stockUpdate = new SDK.Vend.VendWebhook()
                {
                    Type = SDK.Vend.VendWebhook.VendWebhookTypes.InventoryUpdate,
                    Url = GreenspotConfiguration.AppSettings["webhook.inventory.update"].Value,
                    Active = true
                };
                var stockResp = await SDK.Vend.VendWebhook.CreateVendWebhookAsync(stockUpdate, Prefix, await StallApplication.GetAccessTokenAsync(Prefix));
                if (stockResp != null)
                {
                    //save
                    Webhooks.Add(new VendWebhook()
                    {
                        Id = stockUpdate.Id,
                        StallId = Id,
                        Prefix = Prefix,
                        RetailerId = stockUpdate.RetailerId,
                        Type = stockUpdate.Type,
                        Url = stockUpdate.Url,
                        Active = stockUpdate.Active
                    });
                }
                else
                {
                    result.Message = "无法创建库存更新WEBHOOK";
                    return result;
                }
            }

            //set data
            result.Succeeded = true;
            return result;
        }

        public async Task<OperationResult<bool>> UpdateProductById(string id, StallEntities db)
        {
            var result = new OperationResult(false);

            //load product
            var productResult = await VendProduct.GetProductByIdAsync(id, Prefix, await StallApplication.GetAccessTokenAsync(Prefix));
            if (productResult?.Products == null)
            {
                result.Message = string.Format("无法获取商品[{0}]信息", id);
                return result;
            }

            //set products
            var vProducts = productResult?.Products;
            foreach (var vP in vProducts)
            {
                if (vP.Id.Equals(id))
                {
                    var p = Product.ConvertFrom(vP, Id);
                    return (p.Save(db));
                }
            }

            result.Message = string.Format("无法获取商品[{0}]信息", id);
            return result;
        }

        public bool Save()
        {

            using (var db = new StallEntities())
            {
                try
                {
                    db.Set<Stall>().AddOrUpdate(this);
                    //db.Set<StallContact>().AddOrUpdate(Contacts.ToArray());
                    db.Set<Product>().AddOrUpdate(Products.ToArray());
                    db.Set<VendWebhook>().AddOrUpdate(Webhooks.ToArray());
                    db.SaveChanges();
                    return true;
                }
                catch
                {
                    return false;
                }
            }

        }

        #region extend properties
        [NotMapped]
        public IList<Product> SellingProducts
        {
            get
            {
                if (Products == null)
                {
                    return new List<Product>();
                }

                return GetProducts(delegate (Product x)
                {
                    return string.IsNullOrEmpty(x.VariantParentId) && x.Active == true && x.Stock > 0 && x.Price > 0;
                }).ToList();
            }
        }

        [NotMapped]
        private DeliveryPlan _deliveryPlan = null;
        public DeliveryPlan DeliveryPlan
        {
            get
            {
                if (_deliveryPlan == null)
                {
                    if (string.IsNullOrEmpty(DeliveryPlanJsonString))
                    {
                        return new DeliveryPlan();
                    }

                    try
                    {
                        _deliveryPlan = JsonConvert.DeserializeObject<DeliveryPlan>(DeliveryPlanJsonString,
                            new JsonSerializerSettings
                            {
                                TypeNameHandling = TypeNameHandling.Auto
                            });
                    }
                    catch (Exception ex)
                    {
                        return new DeliveryPlan();
                    }
                }

                return _deliveryPlan;
            }
        }
        #endregion

        #region operation
        private IEnumerable<Product> GetProducts(Func<Product, bool> condition)
        {
            if (Products == null)
            {
                return null;
            }
            return Products.Where(condition);
        }

        public int? GetDistance(string destCountryCode, string destCity, string destSuburb)
        {
            if (string.IsNullOrEmpty(destCountryCode) || string.IsNullOrEmpty(destCity)
                || string.IsNullOrEmpty(destSuburb))
            {
                return null;
            }

            if (!CountryId.Equals(destCountryCode) || !City.Equals(destCity))
            {
                return null;
            }

            if (Suburb.Equals(destSuburb))
            {
                return 0;
            }

            return SuburbDistance.GetDistance(CountryId, City, Suburb, destCountryCode, destCity, destSuburb);
        }

        //public IList<DeliverySchedule.DeliveryScheduleItem> GetSchedule(string countryId, string city, string area,
        //                                                                 int nextDays = 7)
        //{
        //    return DeliverySchedule.GetSchedule(countryId, city, area, DefaultOrderAdvancedMinutes ?? 0, nextDays);
        //}

        //public decimal? GetDeliveryFee(string destCountryId, string destCity, string destSuburb, decimal orderAmount = 0)
        //{
        //    return DeliveryFee.Get(CountryId, City, Suburb, destCountryId, destCity, destSuburb, orderAmount);
        //}


        #endregion
    }
}