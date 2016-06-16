using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greenspot.SDK.Vend
{

    public partial class VendRegister
    {
        public static async Task<VendRegisterApiResult> GetRegistersAsync(string prefix, string accessToken)
        {
            var uri = HttpUtility.GetRequestUri(prefix, "registers");
            return await HttpUtility.GetAsync<VendRegisterApiResult>(uri, accessToken);
        }
    }
}
