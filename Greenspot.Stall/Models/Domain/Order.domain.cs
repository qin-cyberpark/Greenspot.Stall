﻿using Greenspot.SDK.Vend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Newtonsoft.Json;
using Greenspot.Configuration;
using System.Data.Entity.Migrations;

namespace Greenspot.Stall.Models
{
    public partial class Order
    {
        public static IList<Order> FindByUserId(string id, StallEntities db)
        {
            if (string.IsNullOrEmpty(id))
            {
                return null;
            }
            return db.Orders.Include(x => x.Stall).Where(x => x.UserId.Equals(id))
                                            .OrderByDescending(x => x.CreateTime).ToList();
        }

        public static Order FindById(int id, StallEntities db)
        {
            return db.Orders.Include(x => x.Stall).Include(x => x.Items).FirstOrDefault(x => x.Id == id);
        }

        public static IList<Order> FindByPaymentId(int paymentId, StallEntities db)
        {
            return db.Orders.Include(x => x.Stall).Include(x => x.Items).Where(x => x.PaymentId == paymentId).ToList();
        }

        public bool Save(StallEntities db)
        {
            db.Orders.AddOrUpdate(this);
            return db.SaveChanges() > 0;
        }

        /// <summary>
        /// Send Order To Vend
        /// </summary>
        /// <param name="db"></param>
        /// <returns></returns>
        private async Task<bool> SendToVend(StallEntities db)
        {
            try
            {
                //var tmpOrder = JsonConvert.DeserializeObject<Order>(JsonString);
                //var tmpStall = Models.Stall.FindById(StallId, db);
                //create vend api object
                var vendSale = new VendRegisterSaleRequest();
                vendSale.InvoiceNumber = Id.ToString();
                vendSale.RegisterId = Stall.RegisterId;
                vendSale.SaleDate = CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
                vendSale.Status = OrderStatus.CLOSED;
                vendSale.TotalPrice = (double)CalcTotalPriceExcludeTax();
                vendSale.TotalTax = (double)CalcTotalTax();
                //vendSale.TaxName = Items[0].Product.TaxName;
                vendSale.RegisterSaleProducts = new List<VendRegisterSaleRequest.RegisterSaleProduct>();
                vendSale.RegisterSalePayments = new List<VendRegisterSaleRequest.RegisterSalePayment>();
                vendSale.Note = Note;
                foreach (var item in Items)
                {
                    var salePdt = new VendRegisterSaleRequest.RegisterSaleProduct()
                    {
                        ProductId = item.ProductId,
                        Quantity = item.Quantity,
                        Price = (double)item.Price,
                        Tax = (double)item.Tax,
                        TaxId = item.TaxId,
                        TaxTotal = (double)item.Tax
                    };

                    salePdt.Attributes.Add(new VendRegisterSaleRequest.Attribute()
                    {
                        Name = "line_note",
                        Value = item.LineNote
                    });

                    vendSale.RegisterSaleProducts.Add(salePdt);
                }

                vendSale.RegisterSalePayments.Add(new VendRegisterSaleRequest.RegisterSalePayment()
                {
                    RetailerPaymentTypeId = Stall.PaymentTypeId,
                    PaymentDate = string.Format("{0:yyyy-MM-dd HH:mm:ss}", Payment.ResponseTime),
                    Amount = (double)Amount
                });

                //do reqeust

                var response = await VendRegisterSale.CreateVendRegisterSalesAsync(vendSale, Stall.Prefix, await StallApplication.GetAccessTokenAsync(Stall.Prefix));
                VendResponse = JsonConvert.SerializeObject(response);
                VendSaleId = response?.RegisterSale?.Id;

                return !string.IsNullOrEmpty(response.RegisterSale.Id) && OrderStatus.CLOSED.Equals(response.RegisterSale.Status);
            }
            catch (Exception ex)
            {
                StallApplication.SysError("[MSG]fail to save to vend", ex);
                return false;
            }
        }

        #region properties
        public bool HasVendSaleCreated
        {
            get
            {
                return !string.IsNullOrEmpty(VendSaleId);
            }
        }
        public decimal CalcTotalPriceExcludeTax()
        {
            decimal ttl = 0;
            foreach (var item in Items)
            {
                ttl += (item.Price ?? 0.0M) * item.Quantity;
            }

            return ttl;
        }

        public decimal CalcTotalTax()
        {
            decimal ttl = 0;
            foreach (var item in Items)
            {
                ttl += (item.Tax ?? 0.0M) * item.Quantity;
            }

            return ttl;
        }

        public decimal CalcTotal()
        {
            return CalcTotalPriceExcludeTax() + CalcTotalTax();
        }
        public decimal CalcTotalCharge()
        {
            return CalcTotal() - StallDiscount - PlatformDiscount;
        }

        public string Summary
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("单号:{0}\r时间:{1:H:mm:ss dd/MM/yyyy}\r金额:{2}\r", Id, CreateTime, ChargeAmount);
                foreach (var item in Items)
                {
                    sb.AppendFormat("{0}@{1:0.00}x{2}\r", item.Name, item.Price, item.Quantity);
                }
                sb.AppendFormat(Note);

                return sb.ToString();
            }
        }
        #endregion
    }
}
