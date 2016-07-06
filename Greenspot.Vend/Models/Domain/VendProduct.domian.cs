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

        public static async Task<VendProductApiResult> GetProductByIdAsync(string productId, string prefix, string accessToken)
        {
            if (string.IsNullOrEmpty(productId))
            {
                return new VendProductApiResult()
                {
                    Products = null
                };
            }
            var uri = HttpUtility.GetRequestUri(prefix, "products/" + productId);
            return await HttpUtility.GetAsync<VendProductApiResult>(uri, accessToken);
        }

        public static async Task<VendProductApiResult> AddProduct(string prefix, string accessToken, VendProduct product)
        {
            if(product == null)
            {
                return new VendProductApiResult()
                {
                    Products = null
                };
            }

            var uri = HttpUtility.GetRequestUri(prefix, "products/");
            return await HttpUtility.PostJSONencodedAsync<VendProductApiResult>(uri, accessToken, product);
        }
    }
}
