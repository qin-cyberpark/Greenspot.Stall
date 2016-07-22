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

        //POST URL ENCODED FORM
        public static async Task<T> PostUrlencodedFormAsync<T>(string requestUri, KeyValuePair<string, string>[] data, KeyValuePair<string, string>[] headers = null)
        {
            try
            {
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);

                if (headers != null)
                {
                    foreach (var h in headers)
                    {
                        requestMessage.Headers.Add(h.Key, h.Value);
                    }
                }
                requestMessage.Content = new FormUrlEncodedContent(data);
                var response = await _httpClient.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode();    // Throw if not a success code.
                var responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseBody);
            }
            catch
            {
                return JsonConvert.DeserializeObject<T>(null);
            }
        }

        //POST URL ENCODED FORM
        public static async Task<T> PostUrlencodedFormAsync<T>(string requestUri, string accessToken, KeyValuePair<string, string>[] data)
        {
            return await PostUrlencodedFormAsync<T>(requestUri, data, 
                                                    new KeyValuePair <string, string>[] {
                                                        new KeyValuePair<string, string>("Authorization",
                                                        string.Format("Bearer {0}",accessToken))});
        }

        //POST JSON
        public static async Task<T> PostJSONencodedAsync<T>(string requestUri, string accessToken, object data)
        {
            return await PostJSONencodedAsync<T>(requestUri, new KeyValuePair<string, string>[] {
                                    new KeyValuePair<string, string>("Authorization",
                                    string.Format("Bearer {0}",accessToken))}, data);
        }

        //POST JSON
        public static async Task<T> PostJSONencodedAsync<T>(
            string requestUri, KeyValuePair<string, string>[] headers, object data)
        {
            try
            {
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, requestUri);
                foreach (var h in headers)
                {
                    requestMessage.Headers.Add(h.Key, h.Value);
                }
                var jsonBody = JsonConvert.SerializeObject(data, Formatting.Indented, new JsonSerializerSettings()
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
                //var jsonBody = JsonConvert.SerializeObject(data);
                requestMessage.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
                var response = await _httpClient.SendAsync(requestMessage);
                response.EnsureSuccessStatusCode();    // Throw if not a success code.
                var responseBody = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<T>(responseBody);
            }
            catch
            {
                return JsonConvert.DeserializeObject<T>(null);
            }
        }

        //GET
        public static async Task<T> GetAsync<T>(string requestUri, string accessToken)
        {
            return await GetAsync<T>(requestUri, new KeyValuePair<string, string>[] {
                                    new KeyValuePair<string, string>("Authorization",
                                    string.Format("Bearer {0}",accessToken))});
        }

        //GET WITH HEADER
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

        //DELETE WITH HEADER
        public static async Task<string> DeleteAsync(string requestUri, KeyValuePair<string, string>[] headers)
        {
            HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Delete, requestUri);
            foreach (var h in headers)
            {
                requestMessage.Headers.Add(h.Key, h.Value);
            }
            var response = await _httpClient.SendAsync(requestMessage);
            response.EnsureSuccessStatusCode();
            var responseBody = await response.Content.ReadAsStringAsync();
            return responseBody;
        }

        //DELETE 
        public static async Task<string> DeleteAsync(string requestUri, string accessToken)
        {
            return await DeleteAsync(requestUri, new KeyValuePair<string, string>[] {
                                    new KeyValuePair<string, string>("Authorization",
                                    string.Format("Bearer {0}",accessToken))});
        }

        public static string GetRequestUri(string prefix, string requestUri)
        {
            return string.Format("https://{0}.vendhq.com/api/{1}", prefix, requestUri);
        }
    }
}
