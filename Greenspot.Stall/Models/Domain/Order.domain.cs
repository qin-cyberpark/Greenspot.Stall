using Greenspot.SDK.Vend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Newtonsoft.Json;
using Greenspot.Configuration;
using System.Data.Entity.Migrations;
using Greenspot.Stall.Utilities;
using Greenspot.Identity;

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
        public async Task<bool> SendToVend()
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
                vendSale.Note = string.Format("#{0}@{1}\n{2}\n{3}\n{4}", Id, PaymentId, Receiver, DeliveryAddress, Note);
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
                    Amount = (double)StallAmount
                });

                //do reqeust
                var response = await VendRegisterSale.CreateVendRegisterSalesAsync(vendSale, Stall.Prefix, await StallApplication.GetAccessTokenAsync(Stall.Prefix));
                //update
                if (!string.IsNullOrEmpty(response.RegisterSale.Id) && OrderStatus.CLOSED.Equals(response.RegisterSale.Status))
                {
                    VendResponse = JsonConvert.SerializeObject(response);
                    VendSaleId = response?.RegisterSale?.Id;
                    return true;
                }
                else
                {
                    StallApplication.SysError("[MSG]fail to save to vend");
                    return false;
                }
            }
            catch (Exception ex)
            {
                StallApplication.SysError("[MSG]fail to save to vend", ex);
                return false;
            }
        }

        public async Task<bool> SendToPrinter()
        {
            try
            {
                if (await PrintHelper.PrintOrderAsync(this))
                {
                    //update
                    return true;
                }
                else
                {
                    StallApplication.SysError("[MSG]fail to print");
                    return false;
                }
            }
            catch (Exception ex)
            {
                StallApplication.SysError("[MSG]fail to print", ex);
                return false;
            }
        }

        public async Task<bool> SendToWechat(IList<string> openids)
        {
            try
            {
                //send message
                var msg = OwnerAlertMessage;
                return await WeChatHelper.SendMessageAsync(openids, msg);
            }
            catch (Exception ex)
            {
                StallApplication.SysError("[MSG]failed to send message", ex);
                return false;
            }
        }


        public async Task Notify(StallEntities db, IList<string> openids)
        {
            //vend
            if (!HasSendToVend)
            {
                HasSendToVend = await SendToVend();
            }

            if (!HasSendToWechat)
            {
                //wechat
                HasSendToWechat = await SendToWechat(openids);
            }

            if (IsPrintOrder && !HasSendToPrinter)
            {
                //print
                HasSendToPrinter = await SendToPrinter();
            }

            //update
            db.SaveChanges();
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
            return CalcTotal() - StallDiscount - PlatformDiscount + PlatformDelivery;
        }

        public string OwnerAlertMessage
        {
            get
            {
                StringBuilder sb = new StringBuilder();
                //store name, order id, order time
                sb.Append($"{Stall.StallName} #{Id}\r{CreateTime:H:mm:ss dd/MMM/yyyy}\r\r");
                //delivery
                sb.Append($"{Receiver}\r{DeliveryAddress}\r");
                //delivery time
                if (DeliveryTimeStart != null && DeliveryTimeEnd != null)
                {
                    sb.Append($"{DeliveryTimeStart:ddd, ddMMM HH:mm}-{DeliveryTimeEnd:HH:mm}\r");
                }
                //items
                sb.Append("\r");
                foreach (var item in Items)
                {
                    sb.Append($"{item.Name}@{item.PriceIncTax:$0.00}x{item.Quantity}\r");
                }
                //total
                sb.Append($"总计:{StallAmount}");
                //note
                if (!string.IsNullOrEmpty(Note))
                {
                    sb.Append($"\r\r备注:{Note}");
                }

                return sb.ToString();
            }
        }
        #endregion
    }
}
