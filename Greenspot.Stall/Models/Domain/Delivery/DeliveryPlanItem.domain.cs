using System;
using System.Collections.Generic;
using System.Linq;
using Greenspot.Stall.Models.Settings;
namespace Greenspot.Stall.Models
{
    public partial class DeliveryPlanItem
    {
        #region properties
        public bool IsPickUp
        {
            get
            {
                return !string.IsNullOrEmpty("PickUpAddress");
            }
        }
        #endregion

        public IList<DateTimePair> GetDateTimePairs(DateTime initDate, int nextDays, bool ignoreExclusiveExtension = true)
        {
            var result = new List<DateTimePair>();
            //foreach (var p in Periods)
            //{
            //    result.AddRange(p.GetDateTimePairs(initDate, nextDays));
            //}

            //if (!ignoreExclusiveExtension && IsExclusive && ExclusiveExtension != null)
            //{
            //    //add exclusive extension
            //    result.ForEach(x =>
            //    {
            //        x.From = x.From.AddMinutes(-ExclusiveExtension.Before);
            //        x.To = x.To.AddMinutes(ExclusiveExtension.After);
            //    });
            //}

            return result;
        }

        public bool IsApplicableToArea(string area)
        {
            //if (Areas == null || Areas.Length == 0)
            //{
            //    return true;
            //}

            //foreach (var def in Areas)
            //{
            //    if (Area.IsApplicable(def, area))
            //    {
            //        return true;
            //    }
            //}

            return false;
        }
    }
}
