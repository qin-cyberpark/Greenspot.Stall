using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greenspot.Stall.Models.ViewModels
{
    public class DeliveryOptionCollectionViewModel
    {
        public DateTime Date { get; set; }

        [JsonProperty(PropertyName = "ApplicableOpts")]
        public IList<DeliveryOption> ApplicableOptions { get; set; } = new List<DeliveryOption>();

        [JsonProperty(PropertyName = "OtherOpts")]
        public IList<DeliveryOption> OtherOptions { get; set; } = new List<DeliveryOption>();
    }
}