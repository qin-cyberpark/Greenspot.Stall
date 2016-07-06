﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greenspot.Stall.Models.ViewModels
{
    public class OrderViewModel
    {
        [JsonProperty(PropertyName = "stall")]
        public OrderStallViewModel Stall { get; set; }

        [JsonProperty(PropertyName = "deliveryOption")]
        public DeliveryOptionViewModel DeliveryOption { get; set; }

        [JsonProperty(PropertyName = "deliveryAddress")]
        public DeliveryAddressViewModel DeliveryAddress { get; set; }

        [JsonProperty(PropertyName = "deliveryFee")]
        public decimal DeliveryFee { get; set; }

        public class OrderStallViewModel
        {
            [JsonProperty(PropertyName = "i")]
            public string Id { get; set; }

            [JsonProperty(PropertyName = "n")]
            public string Name { get; set; }

            [JsonProperty(PropertyName = "qty")]
            public int Quantity { get; set; }

            [JsonProperty(PropertyName = "amt")]
            public decimal Amount { get; set; }

            [JsonProperty(PropertyName = "itms")]
            public IList<OrdeItemViewModel> Items { get; set; }



        }

        public class OrdeItemViewModel
        {
            [JsonProperty(PropertyName = "i")]
            public string Id { get; set; }
            [JsonProperty(PropertyName = "q")]
            public int Quantity { get; set; }
            [JsonProperty(PropertyName = "n")]
            public string Name { get; set; }
            [JsonProperty(PropertyName = "v")]
            public string Variant { get; set; }
            [JsonProperty(PropertyName = "p")]
            public decimal Price { get; set; }
        }

        public class DeliveryOptionViewModel
        {
            [JsonProperty(PropertyName = "Area")]
            public string Area { get; set; }

            [JsonProperty(PropertyName = "From")]
            public DateTime From { get; set; }

            [JsonProperty(PropertyName = "To")]
            public DateTime To { get; set; }

            [JsonProperty(PropertyName = "IsPickUp")]
            public bool IsPickUp { get; set; }

            [JsonProperty(PropertyName = "PickUpAddress")]
            public string PickUpAddress { get; set; }
        }

        public class DeliveryAddressViewModel
        {
            [JsonProperty(PropertyName = "Id")]
            public string Id { get; set; }
        }
    }
}