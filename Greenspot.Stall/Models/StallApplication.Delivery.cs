using Greenspot.Stall.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Greenspot.Stall.Models.Settings;
using Greenspot.Configuration;

namespace Greenspot.Stall
{
    public partial class StallApplication
    {
        private Setting _setting = null;
        public static Setting Setting
        {
            get
            {
                if (_instance._setting == null)
                {
                    using (var file = new StreamReader(GreenspotConfiguration.AppSettings["SettingFile"].Value))
                    {
                        var json = file.ReadToEnd();
                        if (string.IsNullOrEmpty(json))
                        {
                            return new Setting();
                        }
                        try
                        {
                            _instance._setting = JsonConvert.DeserializeObject<Setting>(json,
                                new JsonSerializerSettings
                                {
                                    TypeNameHandling = TypeNameHandling.Auto
                                });
                        }
                        catch (Exception ex)
                        {
                            return new Setting();
                        }
                    }
                }

                return _instance._setting;
            }
        }

        public static IList<DeliveryOrPickupOption> GetDeliveryOptions(Models.Stall stall, DateTime dtStart, int nextDays,
                                                            string area, int? distanceInMeters = null, decimal? orderAmount = null)
        {
            if (nextDays <= 0)
            {
                //stall advanced order days
                nextDays = _instance._setting.MaxAdvancedOrderDays < stall.Setting.MaxAdvancedOrderDays ?
                            _instance._setting.MaxAdvancedOrderDays : stall.Setting.MaxAdvancedOrderDays;
            }

            var options = _instance._setting.Delivery.GetOptions(dtStart, nextDays, null);
            //intersect with stall opening hours

            options = options.Intersect(stall.Setting.OpeningHours.GetDateTimePairs(dtStart, nextDays));

            var result = new List<DeliveryOrPickupOption>();
            foreach (var opt in options)
            {
                if (!Models.Area.IsApplicable(opt.Areas, area))
                {
                    continue;
                }

                var newOpt = DeliveryOrPickupOption.Parse(opt);
                newOpt.IsStoreDelivery = false;
                newOpt.Fee = opt.Calculator.Calculate(opt.From, area, distanceInMeters, orderAmount);
                if (newOpt.Fee != null)
                {
                    result.Add(newOpt);
                }
            }

            return result.Where(x => x.ReferenceTimePonit > dtStart).OrderBy(x => x.From).ToList();
        }
    }
}
