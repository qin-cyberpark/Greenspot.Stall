using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greenspot.Stall.Models
{
    public partial class DeliveryOrPickupOption
    {
        public static DeliveryOrPickupOption Parse(Settings.PickupOption opt)
        {
            return new DeliveryOrPickupOption()
            {
                From = opt.From,
                To = opt.To,
                DivisionTypes = opt.DivisionType,
                DivisionMinutes = opt.DivisionMinutes,
                IsPickUp = true,
                PickUpAddresses = opt.Addresses,
                Fee = 0
            };
        }
        public string AreaString
        {
            get
            {
                var len = Areas.Count;
                var arr = new string[len];
                for (int i = 0; i < len; i++)
                {
                    arr[i] = Areas[i].Substring(Areas[i].LastIndexOf('-') + 1);
                }

                return string.Join(",", arr);
            }
        }
        public bool IsApplicableToArea(string area)
        {
            if (Areas == null || Areas.Count == 0)
            {
                return true;
            }

            foreach (var def in Areas)
            {
                if (Area.IsApplicable(def, area))
                {
                    return true;
                }
            }

            return false;
        }

        //public IList<DeliveryOrPickupOption> Divide()
        //{
        //    var result = new List<DeliveryOrPickupOption>();
        //    var f = From;
        //    var t = f.AddMinutes(DivideMinutes);
        //    t = t < To ? t : To;

        //    if (!IsTimeDivisible)
        //    {
        //        return DivideOverNight(this);
        //    }

        //    while (t < To)
        //    {
        //        result.AddRange(DivideOverNight(new DeliveryOption()
        //        {
        //            From = f,
        //            To = t,
        //            IsTimeDivisible = IsTimeDivisible,
        //            Areas = Areas,
        //            IsPickUp = IsPickUp,
        //            PickUpAddress = PickUpAddress,
        //            Fee = Fee
        //        }));

        //        f = t;
        //        t = t.AddMinutes(OptionDivideMinutes);
        //        t = t < To ? t : To;
        //    }

        //    result.AddRange(DivideOverNight(new DeliveryOption()
        //    {
        //        From = f,
        //        To = t,
        //        IsTimeDivisible = IsTimeDivisible,
        //        Areas = Areas,
        //        IsPickUp = IsPickUp,
        //        PickUpAddress = PickUpAddress,
        //        Fee = Fee
        //    }));

        //    return result;
        //}

        //private IList<DeliveryOrPickupOption> DivideOverNight(DeliveryOrPickupOption option)
        //{
        //    var result = new List<DeliveryOrPickupOption>();

        //    //overnight
        //    while (option.From.Date < option.To.Date)
        //    {
        //        var newTo = new DateTime(option.From.Year, option.From.Month, option.From.Day, 0, 0, 0).AddDays(1);
        //        var newOpt = new DeliveryOrPickupOption()
        //        {
        //            From = option.From,
        //            To = newTo,
        //            IsTimeDivisible = option.IsTimeDivisible,
        //            Areas = option.Areas,
        //            IsPickUp = option.IsPickUp,
        //            PickUpAddress = option.PickUpAddress,
        //            Fee = option.Fee
        //        };

        //        result.Add(newOpt);
        //        option.From = newTo;
        //    };

        //    if (option.From != option.To)
        //    {
        //        result.Add(option);
        //    }
        //    return result;
        //}
    }
}