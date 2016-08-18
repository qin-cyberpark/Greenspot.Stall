using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Greenspot.Stall.Models
{
    public partial class DeliverySchedule
    {
        public class Types
        {
            private Types()
            {

            }
            public const string Directly = "Directly";
        }
        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("Directly")]
        public IList<DeliveryScheduleItem> DirectlyItems { get; set; }

        public partial class DeliveryScheduleItem
        {
            [JsonProperty("From")]
            [JsonConverter(typeof(CustomDateTimeConverter), "dd/MM/yyyy HH:mm:ss")]
            public DateTime From { get; set; }

            [JsonProperty("To")]
            [JsonConverter(typeof(CustomDateTimeConverter), "dd/MM/yyyy HH:mm:ss")]
            public DateTime To { get; set; }

            [JsonProperty("Areas")]
            public string[] Areas { get; set; }

            [JsonProperty("PickUp")]
            public string PickUpAddress { get; set; }
        }
    }
}