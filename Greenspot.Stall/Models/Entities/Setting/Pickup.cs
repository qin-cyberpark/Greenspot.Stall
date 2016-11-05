using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Greenspot.Stall.Models.Settings
{

    public partial class PickupDefinition
    {
        [JsonProperty("Available")]
        public bool Available { get; set; } = false;

        [JsonProperty("Rules")]
        public IList<PickupRule> Rules { get; set; }
    }

    public partial class PickupRule
    {
        [JsonProperty("Addresses")]
        public IList<string> Addresses { get; set; } = new List<string>();

        [JsonProperty("SameAsOpeningHours")]
        public bool SameAsOpeningHours { get; set; }

        [JsonProperty("DateTimes")]
        public DateTimeTerm DateTimes { get; set; }
    }


    public partial class PickupOption : DateTimePair
    {
        public string Address { get; set; }

        public override T Create<T>(DateTime from, DateTime to)
        {
            var obj = base.Create<T>(from, to) as PickupOption;
            if (obj != null)
            {
                obj.Address = Address;
            }
            return obj as T;
        }
    }
}