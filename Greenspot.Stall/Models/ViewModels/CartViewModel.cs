using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greenspot.Stall.Models.ViewModels
{
    public class CartViewModel
    {
        [JsonProperty(PropertyName = "stls")]
        public IList<CartStallViewModel> Stalls { get; set; }

        public class CartStallViewModel
        {
            [JsonProperty(PropertyName = "i")]
            public string Id { get; set; }

            [JsonProperty(PropertyName = "itms")]
            public IList<CartItemViewModel> Items { get; set; }
        }

        public class CartItemViewModel
        {
            [JsonProperty(PropertyName = "i")]
            public string Id { get; set; }
            [JsonProperty(PropertyName = "q")]
            public int Quantity { get; set; }
        }
    }
}
