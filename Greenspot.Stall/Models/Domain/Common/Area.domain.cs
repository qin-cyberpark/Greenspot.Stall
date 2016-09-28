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
            if (string.IsNullOrEmpty(area))
            {
                return false;
            }

            if (string.IsNullOrEmpty(areaDefine))
            {
                return true;
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
    }
}
