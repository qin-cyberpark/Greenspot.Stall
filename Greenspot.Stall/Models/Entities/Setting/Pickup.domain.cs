﻿using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace Greenspot.Stall.Models.Settings
{
    public partial class PickupDefinition
    {
        public IList<PickupOption> GetOptions(DateTime dtDate, int nextDays, DateTimeTerm openingHours)
        {
            var result = new List<PickupOption>();

            if (!Available)
            {
                return result;
            }

            foreach (var r in Rules)
            {
                var opts = r.GetOptions(dtDate, nextDays, openingHours);

                //exclude previous rules
                result.AddRange(opts.Subtract(result));
            }

            //
            return result.OrderBy(x => x.From).ToList();
        }
    }

    public partial class PickupRule
    {
        public IList<PickupOption> GetOptions(DateTime dtDate, int nextDays, DateTimeTerm openingHours)
        {
            var result = new List<PickupOption>();

            IList<DateTimePair> pairs;
            if (SameAsOpeningHours)
            {
                pairs = openingHours.GetDateTimePairs(dtDate, nextDays);
            }
            else if (DateTimes != null)
            {
                pairs = DateTimes.GetDateTimePairs(dtDate, nextDays);
            }
            else
            {
                pairs = new List<DateTimePair>();
            }

            foreach (var p in pairs)
            {
                result.Add(new PickupOption()
                {
                    From = p.From,
                    To = p.To,
                    DivisionType = p.DivisionType,
                    DivisionMinutes = p.DivisionMinutes,
                    Addresses = Addresses
                });
            }

            return result;
        }
    }
}