using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greenspot.SDK.Vend
{
    public partial class VendOutlet
    {
        public static async Task<VendOutletApiResult> GetOutletsAsync(string prefix, string accessToken)
        {
            var uri = HttpUtility.GetRequestUri(prefix, "outlets");
            return await HttpUtility.GetAsync<VendOutletApiResult>(uri, accessToken);
        }
    }
}
