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

            return area.ToUpper().StartsWith(areaDefine.ToUpper());
        }

        public static bool IsApplicable(IList<string> areaDefines, string area)
        {
            if (areaDefines == null || areaDefines.Count == 0)
            {
                return true;
            }

            if (string.IsNullOrEmpty(area))
            {
                return false;
            }

            foreach (var def in areaDefines)
            {
                if (IsApplicable(def, area))
                {
                    return true;
                }
            }

            return false;
        }
    }
}
