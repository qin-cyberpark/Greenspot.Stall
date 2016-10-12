using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greenspot.Stall.Models
{
    public partial class Category
    {
        public static Category Recommend = new Category()
        {
            Identifier = "RECOMMEND",
            Index = 0,
            English = "Popular",
            Chinese = "热门 | 推荐"
        };
        public static Category Others = new Category()
        {
            Identifier = "",
            Index = 999,
            English = "Others",
            Chinese = "其他"
        };

        public static Category Parse(string data)
        {
            data = data.Trim();
            if (string.IsNullOrEmpty(data))
            {
                return Others;
            }

            var arr = data.Trim().Split("::".ToArray(), StringSplitOptions.RemoveEmptyEntries);
            try
            {
                return new Category()
                {
                    Identifier = data,
                    Index = Int32.Parse(arr[0]),
                    English = arr[1],
                    Chinese = arr[2]
                };
            }
            catch
            {
                return null;
            }
        }
    }
}