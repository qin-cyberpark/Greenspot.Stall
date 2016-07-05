using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greenspot.SDK.Vend
{
    public partial class VendRegisterSale
    {
        public static async Task<VendRegisterSaleApiResult> CreateVendRegisterSalesAsync(
            VendRegisterSaleRequest saleReq, string prefix, string accessToken)
        {
            var uri = HttpUtility.GetRequestUri(prefix, "register_sales");
            return await HttpUtility.PostJSONencodedAsync<VendRegisterSaleApiResult>(uri, accessToken, saleReq);
        }
    }
}
