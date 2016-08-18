using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greenspot.Stall.Models
{
    public class DateTimePair
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public bool IsTimeDivisible { get; set; }
        public int OptionDivideMinutes { get; set; }
    }
}