using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace Greenspot.Stall.Models.Settings
{
    public partial class PickupDefinition
    {
        public IList<PickupOption> GetOptions(DateTime startDate, int nextDays, DateTimeTerm openingHours)
        {
            var result = new List<PickupOption>();

            if (!Enabled)
            {
                return result;
            }

            foreach (var r in Rules)
            {
                var opts = r.GetOptions(startDate, nextDays, openingHours);

                //exclude previous rules
                result.AddRange(opts.Subtract(result));
            }

            //
            return result.OrderBy(x => x.From).ToList();
        }
    }

    public partial class PickupRule
    {
        public IList<PickupOption> GetOptions(DateTime startDate, int nextDays, DateTimeTerm openingHours)
        {
            var result = new List<PickupOption>();

            IList<DateTimePair> pairs;
            if (SameAsOpeningHours)
            {
                pairs = openingHours.GetDateTimePairs(startDate, nextDays);
            }
            else if (Hours != null)
            {
                pairs = Hours.GetDateTimePairs(startDate, nextDays);
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