using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greenspot.SDK.Vend
{
    public partial class VendProduct
    {
        public static async Task<VendProductApiResult> GetProductsAsync(string prefix, string accessToken)
        {
            var uri = HttpUtility.GetRequestUri(prefix, "products");
            return await HttpUtility.GetAsync<VendProductApiResult>(uri, accessToken);
        }
    }
}
