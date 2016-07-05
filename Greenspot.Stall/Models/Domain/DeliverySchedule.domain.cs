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
        public partial class FlexItem
        {
            private string _areaString;
            public string AreaString
            {
                get
                {
                    if (string.IsNullOrEmpty(_areaString))
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
        }
        #endregion
    }
}