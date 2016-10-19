using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Greenspot.Stall.Models.Settings
{
    public partial class DateTimeTerm
    {
        public IList<DateTimePair> GetDateTimePairs(DateTime startDate, int nextDays)
        {
            var incList = new List<DateTimePair>();
            var excList = new List<DateTimePair>();
            foreach (var inc in Inclusive)
            {
                incList.AddRange(inc.GetDateTimePairs(startDate, nextDays, DivisionType, DivisionMinutes));
            }
            foreach (var exc in Exclusive)
            {
                excList.AddRange(exc.GetDateTimePairs(startDate, nextDays, DivisionType, DivisionMinutes));
            }

            return incList.Subtract(excList);
        }
    }
}