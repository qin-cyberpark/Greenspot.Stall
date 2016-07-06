using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Greenspot.Stall.Models
{
    public partial class DeliverySchedule
    {

        #region Flex Mode
        public partial class DeliveryScheduleItem
        {
            private string _areaString = null;
            public string AreaString
            {
                get
                {
                    if (string.IsNullOrEmpty(_areaString) && Areas != null)
                    {
                        StringBuilder sb = new StringBuilder();
                        string prefix = "";
                        foreach (var areaId in Areas)
                        {
                            var areaObj = StallApplication.GetArea(areaId);
                            if (areaObj != null)
                            {
                                sb.Append(prefix).Append(areaObj.Name);
                            }
                            prefix = ", ";
                        }
                        _areaString = sb.ToString();
                    }
                    return _areaString;
                }
            }

            public bool IsPickUp
            {
                get
                {
                    return !string.IsNullOrEmpty(PickUpAddress);
                }
            }
        }
        #endregion

        #region Operation
        internal IList<DeliveryScheduleItem> GetSchedule(string countryId, string city, string area, int defaultOrderAdvancedMinutes = 0, int nextDays = 7)
        {
            var wholeArea = string.Format("{0}-{1}-{2}", countryId, city, area);
            switch (Type)
            {
                case Types.Directly:
                    return GetDirectly(wholeArea, defaultOrderAdvancedMinutes, nextDays);
                default:
                    return null;
            }
        }

        private IList<DeliveryScheduleItem> GetDirectly(string area, int defaultOrderAdvancedMinutes, int nextDays)
        {
            if (DirectlyItems == null)
            {
                return null;
            }

            var result = new List<DeliveryScheduleItem>();
            var options = DirectlyItems.Where(x => x.From.Date.Subtract(DateTime.Now.Date).TotalDays <= nextDays).OrderBy(x => x.From).ToList();
            foreach (var opt in options)
            {
                if (opt.From.Subtract(DateTime.Now).TotalMinutes >= defaultOrderAdvancedMinutes &&
                   (opt.IsPickUp || opt.Areas.Contains(area)))
                {
                    result.Add(opt);
                }
            }

            return result;
        }
        #endregion
    }
}