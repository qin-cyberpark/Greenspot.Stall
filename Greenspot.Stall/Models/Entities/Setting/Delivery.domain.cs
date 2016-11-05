using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using System.Linq;

namespace Greenspot.Stall.Models.Settings
{
    public partial class DeliveryDefinition
    {
        public IList<DeliveryOption> GetOptions(DateTime dtStart, int nextDays, DateTimeTerm openingHours)
        {
            var result = new List<DeliveryOption>();

            foreach (var r in Rules)
            {
                IList<DeliveryOption> opts = r.GetOptions(dtStart, nextDays, DefaultCalculator, openingHours);

                //exclude previous rules
                result.AddRange(opts.Subtract(result));
            }

            //
            return result.OrderBy(x => x.From).ToList();
        }
    }

    public partial class DeliveryRule
    {
        public IList<DeliveryOption> GetOptions(DateTime dtStart, int nextDays, DeliveryFeeCalculator defaultCalc, DateTimeTerm openingHours)
        {
            var result = new List<DeliveryOption>();

            var calc = Calculator ?? defaultCalc;
            if (calc == null)
            {
                return result;
            }

            IList<DateTimePair> pairs;
            if (SameAsOpeningHours)
            {
                pairs = openingHours.GetDateTimePairs(dtStart, nextDays);
            }
            else if (DateTimes != null)
            {
                pairs = DateTimes.GetDateTimePairs(dtStart, nextDays);
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
                    ReferenceTimePonitType = p.ReferenceTimePonitType,
                    DivisionType = p.DivisionType,
                    DivisionMinutes = p.DivisionMinutes,
                    Areas = Areas,
                    Calculator = calc
                });
            }

            return result;
        }
    }
}