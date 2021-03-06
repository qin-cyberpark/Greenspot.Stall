﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greenspot.Stall.Models.ViewModels
{
    public class OrderCollectionViewModel
    {
        [JsonProperty(PropertyName = "orders")]
        public IList<OrderViewModel> Orders { get; set; }
    }

    public class OrderViewModel
    {
        [JsonProperty(PropertyName = "i")]
        public int StallId { get; set; }

        [JsonProperty(PropertyName = "n")]
        public string StallName { get; set; }

        [JsonProperty(PropertyName = "qty")]
        public int Quantity { get; set; }

        [JsonProperty(PropertyName = "amt")]
        public decimal Amount { get; set; }

        [JsonProperty(PropertyName = "itms")]
        public IList<OrdeItemViewModel> Items { get; set; }

        [JsonProperty(PropertyName = "selectedDeliveryOrPickupOption")]
        public DeliveryOptionViewModel DeliveryOrPickupOption { get; set; }

        [JsonProperty(PropertyName = "deliveryAddress")]
        public DeliveryAddressViewModel DeliveryAddress { get; set; }

        [JsonProperty(PropertyName = "note")]
        public string Note { get; set; }

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

            [JsonProperty(PropertyName = "IsStoreDelivery")]
            public bool IsStoreDelivery { get; set; }

            [JsonProperty(PropertyName = "PickUpAddress")]
            public string PickUpAddresses { get; set; }

            [JsonProperty(PropertyName = "Fee")]
            public decimal Fee { get; set; }

            public override string ToString()
            {
                if (IsPickUp)
                {
                    return string.Format("Pickup {0:ddd dMMM h:mmtt}-{1:h:mmtt} {2}", From, To, PickUpAddresses[0]);
                }
                else
                {
                    return string.Format("Delivery {0:ddd dMMM h:mmtt}-{1:h:mmtt}", From, To);
                }
            }
        }

        public class DeliveryAddressViewModel
        {
            [JsonProperty(PropertyName = "Code")]
            public string Code { get; set; }
        }
    }
}