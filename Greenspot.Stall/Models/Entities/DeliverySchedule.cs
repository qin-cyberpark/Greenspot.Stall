using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Greenspot.Stall.Models
{
    public partial class DeliverySchedule
    {

        [JsonProperty("Type")]
        public string Type { get; set; }

        [JsonProperty("Flex")]
        public IList<FlexItem> FlexItems { get; set; }

        #region Flex Mode
        public partial class FlexItem
        {
            [JsonProperty("From")]
            public DateTime From { get; set; }

            [JsonProperty("To")]
            public DateTime To { get; set; }

            [JsonProperty("Areas")]
            public string[] Areas { get; set; }
        }
        #endregion
    }
}