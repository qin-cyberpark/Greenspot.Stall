using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greenspot.Stall.Models
{
    public partial class DeliveryOption
    {
        public string AreaString
        {
            get
            {
                var len = Areas.Length;
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
            if (Areas == null || Areas.Length == 0)
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

        public IList<DeliveryOption> Divide()
        {
            var result = new List<DeliveryOption>();
            var f = From;
            var t = f.AddMinutes(OptionDivideMinutes);
            t = t < To ? t : To;

            if (!IsTimeDivisible)
            {
                return DivideOverNight(this);
            }

            while (t < To)
            {
                result.AddRange(DivideOverNight(new DeliveryOption()
                {
                    From = f,
                    To = t,
                    IsTimeDivisible = IsTimeDivisible,
                    Areas = Areas,
                    IsPickUp = IsPickUp,
                    PickUpAddress = PickUpAddress,
                    Fee = Fee
                }));

                f = t;
                t = t.AddMinutes(OptionDivideMinutes);
                t = t < To ? t : To;
            }

            result.AddRange(DivideOverNight(new DeliveryOption()
            {
                From = f,
                To = t,
                IsTimeDivisible = IsTimeDivisible,
                Areas = Areas,
                IsPickUp = IsPickUp,
                PickUpAddress = PickUpAddress,
                Fee = Fee
            }));

            return result;
        }

        private IList<DeliveryOption> DivideOverNight(DeliveryOption option)
        {
            var result = new List<DeliveryOption>();

            //overnight
            while (option.From.Date < option.To.Date)
            {
                var newTo = new DateTime(option.From.Year, option.From.Month, option.From.Day, 0, 0, 0).AddDays(1);
                var newOpt = new DeliveryOption()
                {
                    From = option.From,
                    To = newTo,
                    IsTimeDivisible = option.IsTimeDivisible,
                    Areas = option.Areas,
                    IsPickUp = option.IsPickUp,
                    PickUpAddress = option.PickUpAddress,
                    Fee = option.Fee
                };

                result.Add(newOpt);
                option.From = newTo;
            };

            if (option.From != option.To)
            {
                result.Add(option);
            }
            return result;
        }
    }
}