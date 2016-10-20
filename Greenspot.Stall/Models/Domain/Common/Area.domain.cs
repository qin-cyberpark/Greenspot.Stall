using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Greenspot.Stall.Models
{
    public partial class Area
    {
        public static bool IsApplicable(string areaDefine, string area)
        {

            if (string.IsNullOrEmpty(areaDefine))
            {
                return true;
            }

            if (string.IsNullOrEmpty(area))
            {
                return false;
            }

            var defArr = areaDefine.ToUpper().Split('-');
            var areaArr = area.ToUpper().Split('-');

            if (areaArr.Length < defArr.Length)
            {
                return false;
            }

            for (int i = 0; i < defArr.Length; i++)
            {
                if (!defArr[i].Equals(areaArr[i]))
                {
                    return false;
                }
            }

            return true;
        }

        public static bool Contains(IList<string> areas, string area)
        {
            if (areas != null && areas.Count > 0)
            {
                if (string.IsNullOrEmpty(area))
                {
                    return false;
                }

                foreach (var a in areas)
                {
                    if (area.StartsWith(a))
                    {
                        return true;
                    }
                }

                return false;
            }

            return true;
        }
    }
}
