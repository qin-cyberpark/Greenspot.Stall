using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greenspot.SDK.Vend
{
    public partial class VendPaymentType
    {
        public static async Task<VendPaymentTypeApiResult> GetPaymentTypetsAsync(string prefix, string accessToken)
        {
            var uri = HttpUtility.GetRequestUri(prefix, "payment_types");
            return await HttpUtility.GetAsync<VendPaymentTypeApiResult>(uri, accessToken);
        }
    }
}
