using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Greenspot.SDK.Vend
{
    public partial class VendWebhook
    {
        public static async Task<VendWebhook> CreateVendWebhookAsync(
           VendWebhook webhookReq, string prefix, string accessToken)
        {
            var uri = HttpUtility.GetRequestUri(prefix, "webhooks");
            var data = new KeyValuePair<string, string>[1];
            data[0] = new KeyValuePair<string, string>("data", JsonConvert.SerializeObject(webhookReq));

            return await HttpUtility.PostUrlencodedFormAsync<VendWebhook>(uri, accessToken, data);
        }

        public static async Task<IList<VendWebhook>> GetWebhooksAsync(string prefix, string accessToken)
        {
            var uri = HttpUtility.GetRequestUri(prefix, "webhooks");
            return await HttpUtility.GetAsync<IList<VendWebhook>>(uri, accessToken);
        }

        public static async Task<bool> DeleteWebhookAsync(string prefix, string id, string accessToken)
        {
            var uri = HttpUtility.GetRequestUri(prefix, "webhooks") + "/" + id;
            var result = await HttpUtility.DeleteAsync(uri, accessToken);
            return !string.IsNullOrEmpty(result) && result.Contains("success");
        }
    }
}
