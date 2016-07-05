using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greenspot.Stall.Models.ViewModels
{
    public class CartVM
    {
        [JsonProperty(PropertyName = "stls")]
        public IList<CartStallVM> Stalls { get; set; }

        [JsonProperty(PropertyName = "CurrentStall")]
        public CartStallVM CurrentStall { get; set; }

        public class CartStallVM
        {
            [JsonProperty(PropertyName = "i")]
            public string Id { get; set; }

            [JsonProperty(PropertyName = "itms")]
            public IList<CartItemVM> Items { get; set; }
        }

        public class CartItemVM
        {
            [JsonProperty(PropertyName = "i")]
            public string Id { get; set; }
            [JsonProperty(PropertyName = "q")]
            public int Quantity { get; set; }
        }
    }
}
