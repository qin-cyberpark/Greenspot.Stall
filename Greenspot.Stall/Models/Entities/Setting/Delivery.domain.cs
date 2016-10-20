using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace Greenspot.Stall.Models.Settings
{
    public partial class DeliveryDefinition
    {
        public IList<DeliveryOption> GetOptions(DateTime startDate, int nextDays)
        {
            var result = new List<DeliveryOption>();

            foreach (var r in Rules)
            {
                IList<DeliveryOption> opts = r.GetOptions(startDate, nextDays);

                //exclude previous rules
                result.AddRange(opts.Subtract(result));
            }

            //
            return result.OrderBy(x => x.From).ToList();
        }
    }

    public partial class DeliveryRule
    {
        public IList<DeliveryOption> GetOptions(DateTime startDate, int nextDays)
        {
            var result = new List<DeliveryOption>();

            IList<DateTimePair> pairs;
            if (DateTimes != null)
            {
                pairs = DateTimes.GetDateTimePairs(startDate, nextDays);
            }
            else
            {
                pairs = new List<DateTimePair>();
            }

            foreach (var p in pairs)
            {
                result.Add(new DeliveryOption()
                {
                    From = p.From,
                    To = p.To,
                    DivisionType = p.DivisionType,
                    DivisionMinutes = p.DivisionMinutes,
                    Areas = Areas,
                    Calculator = Calculator
                });
            }

            return result;
        }
    }
}