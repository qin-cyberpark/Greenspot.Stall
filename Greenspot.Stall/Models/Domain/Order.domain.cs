using Greenspot.SDK.Vend;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using Newtonsoft.Json;
using Greenspot.Configuration;

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

        //public async Task<bool> Save(StallEntities db)
        //{
        //    if (await SaveToVend())
        //    {
        //        return SaveToDB(db);
        //    }
        //    return false;
        //}

        private async Task<bool> SaveToVend()
        {
            //create vend api object
            var vendSale = new VendRegisterSaleRequest();
            vendSale.RegisterId = Stall.RegisterId;
            vendSale.SaleDate = CreateTime.ToString("yyyy-MM-dd HH:mm:ss");
            vendSale.Status = OrderStatus.CLOSED;
            vendSale.TotalPrice = (double)CalcTotalPriceExcludeTax();
            vendSale.TotalTax = (double)CalcTotalTax();
            vendSale.TaxName = Items[0].Product.TaxName;
            vendSale.RegisterSaleProducts = new List<VendRegisterSaleRequest.RegisterSaleProduct>();
            vendSale.RegisterSalePayments = new List<VendRegisterSaleRequest.RegisterSalePayment>();
            foreach (var item in Items)
            {
                vendSale.RegisterSaleProducts.Add(new VendRegisterSaleRequest.RegisterSaleProduct()
                {
                    ProductId = item.Product.Id,
                    Quantity = item.Quantity,
                    Price = (double)item.Product.Price,
                    Tax = (double)item.Product.Tax,
                    TaxId = item.Product.TaxId,
                    TaxTotal = (double)item.Product.Tax
                });
            }

            PaidTime = DateTime.Now;
            vendSale.RegisterSalePayments.Add(new VendRegisterSaleRequest.RegisterSalePayment()
            {
                RetailerPaymentTypeId = Stall.PaymentTypeId,
                PaymentDate = PaidTime.Value.ToString("yyyy-MM-dd HH:mm:ss"),
                Amount = (double)TotalCharge
            });

            //do reqeust
            var response = await VendRegisterSale.CreateVendRegisterSalesAsync(vendSale, Stall.Prefix, await StallApplication.GetAccessTokenAsync(Stall.Prefix));
            VendResponse = JsonConvert.SerializeObject(response);
            VendSaleId = response?.RegisterSale?.Id;

            return !string.IsNullOrEmpty(response.RegisterSale.Id) && OrderStatus.CLOSED.Equals(response.RegisterSale.Status);
        }

        public bool Create(StallEntities db)
        {
            var jsonString = JsonConvert.SerializeObject(this, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });

            //set total price
            if (GreenspotConfiguration.Payment.IsFullCharge)
            {
                TotalCharge = CalcTotal();
            }
            else
            {
                TotalCharge = 0.01M;
            }

            db.Orders.Add(this);
            return db.SaveChanges() > 0;
        }

        #region properties
        public decimal CalcTotalPriceExcludeTax()
        {
            decimal ttl = 0;
            foreach (var item in Items)
            {
                ttl += (item.Product.Price ?? 0.0M) * item.Quantity;
            }

            return ttl;
        }

        public decimal CalcTotalTax()
        {
            decimal ttl = 0;
            foreach (var item in Items)
            {
                ttl += (item.Product.Tax ?? 0.0M) * item.Quantity;
            }

            return ttl;
        }

        public decimal CalcTotal()
        {
            return CalcTotalPriceExcludeTax() + CalcTotalTax();
        }
        #endregion
    }
}
