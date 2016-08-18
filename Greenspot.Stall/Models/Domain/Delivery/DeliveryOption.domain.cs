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

            while (t < To)
            {
                result.Add(new DeliveryOption()
                {
                    From = f,
                    To = t,
                    IsTimeDivisible = IsTimeDivisible,
                    Areas = Areas,
                    IsPickUp = IsPickUp,
                    PickUpAddress = PickUpAddress,
                    Fee = Fee
                });

                f = t;
                t = t.AddMinutes(OptionDivideMinutes);
                t = t < To ? t : To;
            }

            result.Add(new DeliveryOption()
            {
                From = f,
                To = t,
                IsTimeDivisible = IsTimeDivisible,
                Areas = Areas,
                IsPickUp = IsPickUp,
                PickUpAddress = PickUpAddress,
                Fee = Fee
            });

            return result;
        }
    }
}