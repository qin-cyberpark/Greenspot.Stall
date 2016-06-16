using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greenspot.SDK.Vend
{
    public partial class VendPaymentType
    {
        [JsonProperty("id")]
        public string Id { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("payment_type_id")]
        public string PaymentTypeId { get; set; }
    }

    public class VendPaymentTypeApiResult
    {
        [JsonProperty("payment_types")]
        public IList<VendPaymentType> PaymentTypes { get; set; }
    }
}
