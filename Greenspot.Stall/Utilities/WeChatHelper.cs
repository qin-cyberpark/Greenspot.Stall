﻿using Greenspot.Configuration;
using Senparc.Weixin.Entities;
using Senparc.Weixin.MP.AdvancedAPIs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public static async Task<bool> SendMessageAsync(string openid, string message, int maxAttamptTime = 3)
        {
            try
            {
                WxJsonResult sendRst = null;
                for (int i = 0; i < maxAttamptTime; i++)
                {
                    sendRst = await CustomApi.SendTextAsync(_appId, openid, message);
                    if (sendRst != null && sendRst.errcode == 0)
                    {
                        break;
                    }
                }

                if (sendRst == null || sendRst.errcode != 0)
                {
                    StallApplication.SysError($"[MSG]failed sending to wechat {openid}:{sendRst.errcode},{sendRst.errmsg}");
                    return false;
                }
                return true;
            }
            catch (Exception ex)
            {
                StallApplication.SysError($"[MSG]failed sending to wechat {openid}:", ex);
                return false;
            }
        }

        public static async Task<bool> SendMessageAsync(IList<string> openids, string message, int maxAttamptTime = 3)
        {
            if (openids == null)
            {
                return false;
            }

            bool succeed = true;
            openids = openids.Distinct().ToList();
            foreach (var openid in openids)
            {
                succeed = succeed && await SendMessageAsync(openid, message, maxAttamptTime);
            }

            return succeed;
        }
    }
}