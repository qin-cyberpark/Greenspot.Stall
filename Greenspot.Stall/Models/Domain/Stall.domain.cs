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

            ////create webhook
            //var webhookResult = await CreateWebhook();
            //if (!webhookResult.Succeeded)
            //{
            //    return webhookResult;
            //}

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
            Email = outlet.Email;
            PhysicalAddress1 = outlet.PhysicalAddress1;
            PhysicalAddress2 = outlet.PhysicalAddress2;
            PhysicalCity = outlet.PhysicalCity;
            PhysicalCountryId = outlet.PhysicalCountryId;
            PhysicalPostcode = outlet.PhysicalPostcode;
            PhysicalState = outlet.PhysicalState;
            PhysicalSuburb = outlet.PhysicalSuburb;
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

            //load payment types
            var paytypeResult = await VendPaymentType.GetPaymentTypetsAsync(Prefix, await StallApplication.GetAccessTokenAsync(Prefix));
            if (paytypeResult?.PaymentTypes == null || paytypeResult.PaymentTypes.Count == 0)
            {
                result.Message = "无法同步VEND PAYMENT TYPE信息";
                return result;
            }

            //set payment type
            string stallPaymentTypeName = "CASH";
            string stallPaymentTypeId = null;
            foreach (var pt in paytypeResult.PaymentTypes)
            {
                if (pt.Name.ToUpper().Equals(stallPaymentTypeName))
                {
                    stallPaymentTypeId = pt.Id;
                    break;
                }
            }

            if (string.IsNullOrEmpty(stallPaymentTypeId))
            {
                result.Message = string.Format("找不到名为{0}的VEND PAYMENT TYPE", stallPaymentTypeName);
                return result;
            }
            PaymentTypeId = stallPaymentTypeId;

            //load registers
            var registerResult = await VendRegister.GetRegistersAsync(Prefix, await StallApplication.GetAccessTokenAsync(Prefix));
            if (registerResult?.Registers == null || registerResult.Registers.Count == 0)
            {
                result.Message = "无法同步VEND REGISTER信息";
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
                result.Message = "无法同步VEND商品信息";
                return result;
            }

            //set products
            Products.Clear();

            //var products = productResult?.Products.OrderBy(x => x.VariantParentId).ToList();
            var vProducts = productResult?.Products;
            foreach (var vP in vProducts)
            {
                Products.Add(Product.ConvertFrom(vP, Id));
            }

            //set data
            result.Succeeded = true;
            return result;
        }

        private async Task<OperationResult<bool>> CreateWebhook()
        {
            var result = new OperationResult(false);

            //create product update webhook
            var pdtUpdate = new SDK.Vend.VendWebhookRequest()
            {
                Type = SDK.Vend.VendWebhook.VendWebhookTypes.ProductUpdate,
                Url = GreenspotConfiguration.AppSettings["webhook.product.update"].Value
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

            //create inventory update webhook
            var stockUpdate = new SDK.Vend.VendWebhookRequest()
            {
                Type = SDK.Vend.VendWebhook.VendWebhookTypes.InventoryUpdate,
                Url = GreenspotConfiguration.AppSettings["webhook.inventory.update"].Value
            };
            var stockResp = await SDK.Vend.VendWebhook.CreateVendWebhookAsync(pdtUpdate, Prefix, await StallApplication.GetAccessTokenAsync(Prefix));
            if (stockResp != null)
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
                result.Message = "无法创建库存更新WEBHOOK";
                return result;
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
                db.Set<Stall>().AddOrUpdate(this);
                db.Set<StallContact>().AddOrUpdate(Contacts.ToArray());
                db.Set<Product>().AddOrUpdate(Products.ToArray());
                db.Set<VendWebhook>().AddOrUpdate(Webhooks.ToArray());
                db.SaveChanges();
                return true;
            }
        }

        #region extend properties
        [NotMapped]
        public IList<Product> BaseProducts
        {
            get
            {
                if (Products == null)
                {
                    return null;
                }

                return GetProducts(delegate (Product x)
                {
                    return string.IsNullOrEmpty(x.VariantParentId);
                }).ToList();
            }
        }

        [NotMapped]
        private DeliveryFee _deliveryFee = null;
        public DeliveryFee DeliveryFee
        {
            get
            {
                if (_deliveryFee == null)
                {
                    if (string.IsNullOrEmpty(DeliveryFeeJsonString))
                    {
                        return null;
                    }
                    try
                    {
                        _deliveryFee = JsonConvert.DeserializeObject<DeliveryFee>(DeliveryFeeJsonString);
                    }
                    catch
                    {
                        return null;
                    }
                }

                return _deliveryFee;

            }
        }

        [NotMapped]
        private DeliverySchedule _deliverySchedule = null;
        public DeliverySchedule DeliverySchedule
        {
            get
            {
                if (_deliverySchedule == null)
                {
                    if (string.IsNullOrEmpty(DeliveryScheduleJsonString))
                    {
                        return null;
                    }

                    try
                    {
                        _deliverySchedule = JsonConvert.DeserializeObject<DeliverySchedule>(DeliveryScheduleJsonString);

                    }
                    catch
                    {
                        return null;
                    }
                }

                return _deliverySchedule;
            }
        }

        private IEnumerable<Product> GetProducts(Func<Product, bool> condition)
        {
            if (Products == null)
            {
                return null;
            }
            return Products.Where(condition).Where(x => !x.Handle.Equals("vend-discount") && (x.Active ?? false));
        }
        #endregion

        #region operation
        public IList<DeliverySchedule.DeliveryScheduleItem> GetSchedule(string countryId, string city, string area,
                                                                         int nextDays = 7)
        {
            return DeliverySchedule.GetSchedule(countryId, city, area, DefaultOrderAdvancedMinutes ?? 0, nextDays);
        }

        public decimal? GetDeliveryFee(string destCountryId, string destCity, string destSuburb, decimal orderAmount = 0)
        {
            return DeliveryFee.Get(PhysicalCountryId, PhysicalCity, PhysicalSuburb, destCountryId, destCity, destSuburb, orderAmount);
        }
        #endregion
    }
}