using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greenspot.SDK.Vend
{
    public partial class Supplier
    {
        public static async Task<SupplierApiResult> GetSuppliersAsync(string prefix, string accessToken)
        {
            var uri = HttpUtility.GetRequestUri(prefix, "supplier");
            return await HttpUtility.GetAsync<SupplierApiResult>(uri, accessToken);
        }
    }
}
