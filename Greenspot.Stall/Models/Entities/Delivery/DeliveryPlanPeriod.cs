using Newtonsoft.Json;
using System.Collections.Generic;

namespace Greenspot.Stall.Models
{
    public partial class DeliveryPlanPeriod
    {
        //1-31
        [JsonProperty("DaysOfMonth")]
        public IList<string> DaysOfMonth
        { get; set; }

        //0-6 0 is Sunday, 6 is Saturday, 7 is also Sunday
        [JsonProperty("DaysOfWeek")]
        public IList<string> DaysOfWeek { get; set; }

        //d/M/yyyy
        [JsonProperty("Dates")]
        public IList<string> Dates { get; set; }

        //HH:mm
        [JsonProperty("Times")]
        public IList<string> Times { get; set; }

        [JsonProperty("IsTimeDivisible")]
        public bool IsTimeDivisible { get; set; } = false;

        [JsonProperty("OptionDivideMinutes")]
        public int OptionDivideMinutes { get; set; }

        [JsonIgnore]
        public const string DateFormatString = "d/M/yyyy";

        [JsonIgnore]
        public const string TimeFormatString = "HH:mm";
    }
}