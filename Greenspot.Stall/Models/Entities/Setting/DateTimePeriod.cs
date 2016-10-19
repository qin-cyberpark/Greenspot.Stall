using Newtonsoft.Json;
using System.Collections.Generic;

namespace Greenspot.Stall.Models.Settings
{
    public partial class DateTimePeriod
    {
        //1-31
        [JsonProperty("DaysOfMonth")]
        public IList<string> DaysOfMonth { get; set; } = new List<string>();

        //0-6 0 is Sunday, 6 is Saturday, 7 is also Sunday
        [JsonProperty("DaysOfWeek")]
        public IList<string> DaysOfWeek { get; set; } = new List<string>();

        //yyyy/M/d
        [JsonProperty("Dates")]
        public IList<string> Dates { get; set; } = new List<string>();

        //HH:mm
        [JsonProperty("Times")]
        public IList<string> Times { get; set; } = new List<string>();

        [JsonIgnore]
        public const string DateFormatString = "yyyy/M/d";

        [JsonIgnore]
        public const string TimeFormatString = "H:mm";
    }
}