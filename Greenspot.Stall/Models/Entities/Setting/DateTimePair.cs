using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greenspot.Stall.Models.Settings
{
    public partial class DateTimePair
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public TimeDivisionTypes DivisionType { get; set; }
        public int DivisionMinutes { get; set; }
    }
}