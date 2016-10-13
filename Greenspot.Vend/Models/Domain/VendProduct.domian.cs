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
            var query = new SortedList<string, string> { { "order_by", "id" } };

            var uri = HttpUtility.GetRequestUri(prefix, "products", query);
            var result = await HttpUtility.GetAsync<VendProductApiResult>(uri, accessToken);
            VendProductApiResult conResult = result;
            while (result.Pagination != null && conResult.Pagination.Page < conResult.Pagination.Pages)
            {
                //get continuous pages
                query["page"] = (conResult.Pagination.Page + 1).ToString();
                uri = HttpUtility.GetRequestUri(prefix, "products", query);
                conResult = await HttpUtility.GetAsync<VendProductApiResult>(uri, accessToken);
                if (conResult?.Products == null)
                {
                    return null;
                }
                else
                {
                    foreach (var p in conResult.Products)
                    {
                        result.Products.Add(p);
                    }
                }
            }

            return result;
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
            if (product == null)
            {
                return new VendProductApiResult()
                {
                    Product = null
                };
            }

            var uri = HttpUtility.GetRequestUri(prefix, "products/");
            return await HttpUtility.PostJSONencodedAsync<VendProductApiResult>(uri, accessToken, product);
        }
    }
}
