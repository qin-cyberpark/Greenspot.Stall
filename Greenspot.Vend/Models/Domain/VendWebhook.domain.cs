using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greenspot.SDK.Vend
{
    public partial class VendWebhook
    {
        public static async Task<VendWebhook> CreateVendWebhookAsync(
           VendWebhookRequest webhookReq, string prefix, string accessToken)
        {
            var uri = HttpUtility.GetRequestUri(prefix, "webhooks");
            return await HttpUtility.PostJSONencodedAsync<VendWebhook>(uri, accessToken, webhookReq);
        }
    }
}
