using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Greenspot.Identity;
using Greenspot.Identity.OAuth.WeChat;

namespace Greenspot.Identity.Extensions
{
    public static class GreenspotUserExtension
    {
        // This is the extension method.
        // The first parameter takes the "this" modifier
        // and specifies the type for which the method is defined.
        public static bool HasSubscribed(this GreenspotUser user)
        {
            return bool.Parse(user.SnsInfos[WeChatClaimTypes.Subscribed].InfoValue);
        }
    }
}