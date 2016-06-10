using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
namespace Greenspot.SDK.Vend
{
    public static class HttpUtility
    {
        private static HttpClient _httpClient = new HttpClient();
        public static async Task<T> PostUrlencodedFormAsync<T>(string requestUri, KeyValuePair<string, string>[] data)
        {
            var content = new FormUrlEncodedContent(data);
            var response = await _httpClient.PostAsync(requestUri, content);
            response.EnsureSuccessStatusCode();    // Throw if not a success code.
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseBody);
        }

        public static async Task<T> GetAsync<T>(string requestUri, string accessToken)
        {
            return await GetAsync<T>(requestUri, new KeyValuePair<string, string>[] {
                                    new KeyValuePair<string, string>("Authorization", 
                                    string.Format("Bearer {0}",accessToken))});
        }

        public static async Task<T> GetAsync<T>(string requestUri, KeyValuePair<string, string>[] headers)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Get, requestUri);
            foreach (var h in headers)
            {
                requestMessage.Headers.Add(h.Key, h.Value);
            }
            var response = await _httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<T>(responseBody);
        }

        public static string GetRequestUri(string prefix, string requestUri)
        {
            return string.Format("https://{0}.vendhq.com/api/{1}", prefix, requestUri);
        }
    }
}
