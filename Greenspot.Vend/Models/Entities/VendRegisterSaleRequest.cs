using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greenspot.SDK.Vend
{
    public partial class VendRegisterSaleRequest
    {
        public class Attribute
        {
            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("value")]
            public string Value { get; set; }
        }

        public class RegisterSaleProduct
        {
         
            [JsonProperty("product_id")]
            public string ProductId { get; set; }

         
            [JsonProperty("quantity")]
            public int Quantity { get; set; }

            [JsonProperty("price")]
            public double Price { get; set; }

            [JsonProperty("tax")]
            public double Tax { get; set; }

            [JsonProperty("tax_id")]
            public string TaxId { get; set; }

            [JsonProperty("tax_total")]
            public double TaxTotal { get; set; }

            [JsonProperty("attributes")]
            public IList<Attribute> Attributes { get; set; }
        }

        public class RegisterSalePayment
        {

            [JsonProperty("retailer_payment_type_id")]
            public string RetailerPaymentTypeId { get; set; }

            [JsonProperty("amount")]
            public double Amount { get; set; }

            [JsonProperty("payment_date")]
            public string PaymentDate { get; set; }
        }

        [JsonProperty("register_id")]
        public string RegisterId { get; set; }

    
        [JsonProperty("customer_id")]
        public string CustomerId { get; set; }

        [JsonProperty("sale_date")]
        public string SaleDate { get; set; }

        [JsonProperty("user_name")]
        public string UserName { get; set; } = "admin";

        [JsonProperty("total_price")]
        public double TotalPrice { get; set; }

        [JsonProperty("total_tax")]
        public double TotalTax { get; set; }

        [JsonProperty("tax_name")]
        public string TaxName { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("invoice_number")]
        public string InvoiceNumber { get; set; }

        [JsonProperty("invoice_sequence")]
        public string InvoiceSequence { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }

        [JsonProperty("register_sale_products")]
        public IList<RegisterSaleProduct> RegisterSaleProducts { get; set; }

        [JsonProperty("register_sale_payments")]
        public IList<RegisterSalePayment> RegisterSalePayments { get; set; } = new List<RegisterSalePayment>();
    }
}
