using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greenspot.Stall.Models.ViewModels
{
    public class DeliveryOptionCollectionViewModel
    {
        //public DateTime Date { get; set; }

        //[JsonProperty(PropertyName = "ApplicableOpts")]
        //public IList<DeliveryOrPickupOption> ApplicableOptions { get; set; } = new List<DeliveryOrPickupOption>();

        //[JsonProperty(PropertyName = "OtherOpts")]
        //public IList<DeliveryOrPickupOption> OtherOptions { get; set; } = new List<DeliveryOrPickupOption>();

        [JsonProperty(PropertyName = "Options")]
        public IList<DeliveryOrPickupOption> Options { get; set; } = new List<DeliveryOrPickupOption>();
    }

    public class PickupOptionGroupViewModel
    {
        [JsonProperty(PropertyName = "Groups")]
        public IList<PickupOptionGroupViewModel> Groups { get; set; } = new List<PickupOptionGroupViewModel>();

        [JsonProperty(PropertyName = "Options")]
        public IList<DeliveryOrPickupOption> Options { get; set; } = new List<DeliveryOrPickupOption>();
    }
}