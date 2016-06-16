using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greenspot.SDK.Vend
{
    public partial class VendRegister
    {

        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("outlet_id")]
        public string OutletId { get; set; }

        [JsonProperty("button_layout_id")]
        public string ButtonLayoutId { get; set; }

        [JsonProperty("print_receipt")]
        public string PrintReceipt { get; set; }

        [JsonProperty("email_receipt")]
        public string EmailReceipt { get; set; }

        [JsonProperty("ask_for_note_on_save")]
        public string AskForNoteOnSave { get; set; }

        [JsonProperty("print_note_on_receipt")]
        public string PrintNoteOnReceipt { get; set; }

        [JsonProperty("ask_for_user_on_sale")]
        public string AskForUserOnSale { get; set; }

        [JsonProperty("show_discounts_on_receipt")]
        public string ShowDiscountsOnReceipt { get; set; }

        [JsonProperty("receipt_header")]
        public string ReceiptHeader { get; set; }

        [JsonProperty("receipt_barcoded")]
        public string ReceiptBarcoded { get; set; }

        [JsonProperty("receipt_footer")]
        public string ReceiptFooter { get; set; }

        [JsonProperty("receipt_style_class")]
        public string ReceiptStyleClass { get; set; }

        [JsonProperty("invoice_prefix")]
        public string InvoicePrefix { get; set; }

        [JsonProperty("invoice_suffix")]
        public string InvoiceSuffix { get; set; }

        [JsonProperty("invoice_sequence")]
        public int InvoiceSequence { get; set; }

        [JsonProperty("register_open_count_sequence")]
        public string RegisterOpenCountSequence { get; set; }

        [JsonProperty("register_open_sequence_id")]
        public string RegisterOpenSequenceId { get; set; }

        [JsonProperty("register_open_time")]
        public string RegisterOpenTime { get; set; }

        [JsonProperty("register_close_time")]
        public string RegisterCloseTime { get; set; }

        [JsonProperty("cash_managed_payment_type_id")]
        public object CashManagedPaymentTypeId { get; set; }

        [JsonProperty("is_quick_keys_enabled")]
        public bool IsQuickKeysEnabled { get; set; }

        [JsonProperty("quick_key_template_id")]
        public object QuickKeyTemplateId { get; set; }
    }

    public class VendRegisterApiResult
    {
        [JsonProperty("registers")]
        public IList<VendRegister> Registers { get; set; }
    }

}
