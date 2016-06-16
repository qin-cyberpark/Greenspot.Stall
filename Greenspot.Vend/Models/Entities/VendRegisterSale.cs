using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greenspot.SDK.Vend
{
    public partial class VendRegisterSale
    {
        public class Contact
        {
            [JsonProperty("company_name")]
            public string CompanyName { get; set; }

            [JsonProperty("phone")]
            public string Phone { get; set; }

            [JsonProperty("email")]
            public string Email { get; set; }
        }

        public class CustomerSection
        {
            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("customer_code")]
            public string CustomerCode { get; set; }

            [JsonProperty("customer_group_id")]
            public string CustomerGroupId { get; set; }

            [JsonProperty("customer_group_name")]
            public string CustomerGroupName { get; set; }

            [JsonProperty("first_name")]
            public string FirstName { get; set; }

            [JsonProperty("last_name")]
            public string LastName { get; set; }

            [JsonProperty("company_name")]
            public string CompanyName { get; set; }

            [JsonProperty("phone")]
            public string Phone { get; set; }

            [JsonProperty("mobile")]
            public string Mobile { get; set; }

            [JsonProperty("fax")]
            public string Fax { get; set; }

            [JsonProperty("email")]
            public string Email { get; set; }

            [JsonProperty("twitter")]
            public string Twitter { get; set; }

            [JsonProperty("website")]
            public string Website { get; set; }

            [JsonProperty("physical_address1")]
            public string PhysicalAddress1 { get; set; }

            [JsonProperty("physical_address2")]
            public string PhysicalAddress2 { get; set; }

            [JsonProperty("physical_suburb")]
            public string PhysicalSuburb { get; set; }

            [JsonProperty("physical_city")]
            public string PhysicalCity { get; set; }

            [JsonProperty("physical_postcode")]
            public string PhysicalPostcode { get; set; }

            [JsonProperty("physical_state")]
            public string PhysicalState { get; set; }

            [JsonProperty("physical_country_id")]
            public string PhysicalCountryId { get; set; }

            [JsonProperty("postal_address1")]
            public string PostalAddress1 { get; set; }

            [JsonProperty("postal_address2")]
            public string PostalAddress2 { get; set; }

            [JsonProperty("postal_suburb")]
            public string PostalSuburb { get; set; }

            [JsonProperty("postal_city")]
            public string PostalCity { get; set; }

            [JsonProperty("postal_postcode")]
            public string PostalPostcode { get; set; }

            [JsonProperty("postal_state")]
            public string PostalState { get; set; }

            [JsonProperty("postal_country_id")]
            public string PostalCountryId { get; set; }

            [JsonProperty("updated_at")]
            public string UpdatedAt { get; set; }

            [JsonProperty("deleted_at")]
            public string DeletedAt { get; set; }

            [JsonProperty("balance")]
            public double Balance { get; set; }

            [JsonProperty("year_to_date")]
            public string YearToDate { get; set; }

            [JsonProperty("date_of_birth")]
            public string DateOfBirth { get; set; }

            [JsonProperty("sex")]
            public string Sex { get; set; }

            [JsonProperty("custom_field_1")]
            public string CustomField1 { get; set; }

            [JsonProperty("custom_field_2")]
            public string CustomField2 { get; set; }

            [JsonProperty("custom_field_3")]
            public string CustomField3 { get; set; }

            [JsonProperty("custom_field_4")]
            public string CustomField4 { get; set; }

            [JsonProperty("note")]
            public string Note { get; set; }

            [JsonProperty("contact")]
            public Contact Contact { get; set; }
        }

        public class Attribute
        {

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("value")]
            public string Value { get; set; }
        }

        public class RegisterSaleProduct
        {

            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("product_id")]
            public string ProductId { get; set; }

            [JsonProperty("register_id")]
            public string RegisterId { get; set; }

            [JsonProperty("sequence")]
            public string Sequence { get; set; }

            [JsonProperty("handle")]
            public string Handle { get; set; }

            [JsonProperty("sku")]
            public string Sku { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("quantity")]
            public int Quantity { get; set; }

            [JsonProperty("price")]
            public double Price { get; set; }

            [JsonProperty("cost")]
            public double Cost { get; set; }

            [JsonProperty("price_set")]
            public double PriceSet { get; set; }

            [JsonProperty("discount")]
            public double Discount { get; set; }

            [JsonProperty("loyalty_value")]
            public int LoyaltyValue { get; set; }

            [JsonProperty("tax")]
            public double Tax { get; set; }

            [JsonProperty("tax_id")]
            public string TaxId { get; set; }

            [JsonProperty("tax_name")]
            public string TaxName { get; set; }

            [JsonProperty("tax_rate")]
            public double TaxRate { get; set; }

            [JsonProperty("tax_total")]
            public double TaxTotal { get; set; }

            [JsonProperty("price_total")]
            public double PriceTotal { get; set; }

            [JsonProperty("display_retail_price_tax_inclusive")]
            public double DisplayRetailPriceTaxInclusive { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }

            [JsonProperty("attributes")]
            public IList<Attribute> Attributes { get; set; }
        }

        public class TotalSection
        {

            [JsonProperty("total_tax")]
            public double TotalTax { get; set; }

            [JsonProperty("total_price")]
            public double TotalPrice { get; set; }

            [JsonProperty("total_payment")]
            public double TotalPayment { get; set; }

            [JsonProperty("total_to_pay")]
            public double TotalToPay { get; set; }
        }

        public class RegisterSalePayment
        {

            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("payment_type_id")]
            public string PaymentTypeId { get; set; }

            [JsonProperty("register_id")]
            public string RegisterId { get; set; }

            [JsonProperty("retailer_payment_type_id")]
            public string RetailerPaymentTypeId { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("label")]
            public string Label { get; set; }

            [JsonProperty("payment_date")]
            public DateTime PaymentDate { get; set; }

            [JsonProperty("amount")]
            public double Amount { get; set; }
        }

        public class TaxItem
        {

            [JsonProperty("id")]
            public string Id { get; set; }

            [JsonProperty("tax")]
            public double Tax { get; set; }

            [JsonProperty("name")]
            public string Name { get; set; }

            [JsonProperty("rate")]
            public double Rate { get; set; }
        }


        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("source")]
        public string Source { get; set; }

        [JsonProperty("source_id")]
        public string SourceId { get; set; }

        [JsonProperty("register_id")]
        public string RegisterId { get; set; }

        [JsonProperty("market_id")]
        public string MarketId { get; set; }

        [JsonProperty("customer_id")]
        public string CustomerId { get; set; }

        [JsonProperty("customer_name")]
        public string CustomerName { get; set; }

        [JsonProperty("customer")]
        public CustomerSection Customer { get; set; }

        [JsonProperty("user_id")]
        public string UserId { get; set; }

        [JsonProperty("user_name")]
        public string UserName { get; set; }

        [JsonProperty("sale_date")]
        public string SaleDate { get; set; }

        [JsonProperty("created_at")]
        public string CreatedAt { get; set; }

        [JsonProperty("updated_at")]
        public string UpdatedAt { get; set; }

        [JsonProperty("total_price")]
        public double TotalPrice { get; set; }

        [JsonProperty("total_cost")]
        public double TotalCost { get; set; }

        [JsonProperty("total_tax")]
        public double TotalTax { get; set; }

        [JsonProperty("tax_name")]
        public string TaxName { get; set; }

        [JsonProperty("note")]
        public string Note { get; set; }

        [JsonProperty("status")]
        public string Status { get; set; }

        [JsonProperty("short_code")]
        public string ShortCode { get; set; }

        [JsonProperty("invoice_number")]
        public string InvoiceNumber { get; set; }

        [JsonProperty("return_for")]
        public string ReturnFor { get; set; }

        [JsonProperty("register_sale_products")]
        public IList<RegisterSaleProduct> RegisterSaleProducts { get; set; }

        [JsonProperty("totals")]
        public TotalSection Totals { get; set; }

        [JsonProperty("register_sale_payments")]
        public IList<RegisterSalePayment> RegisterSalePayments { get; set; }

        [JsonProperty("taxes")]
        public IList<TaxItem> Taxes { get; set; }

    }

    public class VendRegisterSaleApiResult
    {
        [JsonProperty("register_sale")]
        public VendRegisterSale RegisterSale { get; set; }
    }
}
