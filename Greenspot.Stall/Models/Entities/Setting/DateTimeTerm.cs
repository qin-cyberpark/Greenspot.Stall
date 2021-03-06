﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Greenspot.Stall.Models.Settings
{
    public enum TimeDivisionTypes : int { Undivisible = 0, DivideToPeriod, DivideToTime }
    public enum ReferenceTimePonitTypes : int { ByFrom = 0, ByTo }

    public partial class DateTimeTerm
    {
        [JsonProperty("Inclusive")]
        public IList<DateTimePeriod> Inclusive { get; set; } = new List<DateTimePeriod>();

        [JsonProperty("Exclusive")]
        public IList<DateTimePeriod> Exclusive { get; set; } = new List<DateTimePeriod>();

        [JsonProperty("ReferenceTimePonitType")]
        public ReferenceTimePonitTypes ReferenceTimePonitType { get; set; } = ReferenceTimePonitTypes.ByTo;

        [JsonProperty("DivisionType")]
        public TimeDivisionTypes DivisionType { get; set; }

        [JsonProperty("DivisionMinutes")]
        public int DivisionMinutes { get; set; } = 60;
    }
}