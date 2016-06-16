using Greenspot.Configuration;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP.AdvancedAPIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greenspot.Stall.Utilities
{
    public class WeChatHelper
    {
        private static string _appId = GreenspotConfiguration.AccessAccounts["wechat"].Id;
        /// <summary>
        /// send image to openid
        /// </summary>
        /// <param name="openid"></param>
        /// <param name="fileByte"></param>
        /// <returns></returns>
        public static bool SendMessage(string openid, string message, int maxAttamptTime = 3)
        {
            try
            {
                WxJsonResult sendRst = null;
                for (int i = 0; i < maxAttamptTime; i++)
                {
                    sendRst = CustomApi.SendText(_appId, openid, message);
                    if (sendRst != null && sendRst.errcode == 0)
                    {
                        break;
                    }
                }
                if (sendRst == null || sendRst.errcode != 0)
                {
                    return false;
                }
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}