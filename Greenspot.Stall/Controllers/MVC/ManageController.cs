using Greenspot.Stall.Models;
using Senparc.Weixin.MP.AdvancedAPIs;
using Senparc.Weixin.MP.Containers;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Greenspot.Identity.OAuth.WeChat;

namespace Greenspot.Stall.Controllers.MVC
{
    public class ManageController : Controller
    {
        private StallEntities _db = new StallEntities();

        // GET: Manage
        public ActionResult Stall(int id)
        {
            var stall = Models.Stall.FindById(id, _db);
            if (stall == null)
            {
                stall = new Models.Stall();
            }
            return View(stall);
        }

        public async Task<ActionResult> Init(int id)
        {
            var stall = Models.Stall.FindById(id, _db);
            if (stall != null && string.IsNullOrEmpty(stall.PaymentTypeId) && string.IsNullOrEmpty(stall.DeliveryProductId))
            {
                var result = await stall.Init();
                if (result.Succeeded)
                {
                    return Redirect("/manage/stall/" + stall.Prefix);
                }
                else
                {
                    return Content(result.Message);
                }
            }

            return Content("stall id=" + id + "不存在");
        }

        public async Task<ActionResult> MultiInit(int id)
        {
            var result = await Models.Stall.InitMultiStall(id, _db);
            if (!result.Succeeded)
            {
                return Content(result.Message);
            }
            else
            {
                return Content("done");
            }
        }

        #region UpdateUserWechatInfo
        public ActionResult UpdateUserWechatInfo()
        {
            using (var db = new IdentityEntities())
            {
                var page = 0;
                var pageSize = 100;

                //init 
                AccessTokenContainer.Register(Configuration.GreenspotConfiguration.AccessAccounts["wechat"].Id,
                                    Configuration.GreenspotConfiguration.AccessAccounts["wechat"].Secret);

                //take all users
                var users = db.UserSnsInfos.Where(x => WeChatClaimTypes.OpenId.Equals(x.InfoKey)).ToList();
                if (users.Count == 0)
                {
                    return Content("NG");
                }

                var user100 = users.Skip(pageSize * page++).Take(pageSize).ToList();

                //take 100 user
                while (user100.Count > 0)
                {
                    //openId, userId
                    var openIdDic = new SortedList<string, string>();

                    var reqData = new List<Senparc.Weixin.MP.AdvancedAPIs.User.BatchGetUserInfoData>();

                    //build user info data
                    foreach (var u in users)
                    {
                        openIdDic.Add(u.InfoValue, u.UserId);
                        reqData.Add(new Senparc.Weixin.MP.AdvancedAPIs.User.BatchGetUserInfoData()
                        {
                            lang = Senparc.Weixin.Language.zh_CN,
                            openid = u.InfoValue
                        });
                    }

                    //pull data
                    var result = UserApi.BatchGetUserInfo(Configuration.GreenspotConfiguration.AccessAccounts["wechat"].Id, reqData);
                    if (result.user_info_list != null && result.user_info_list.Count > 0)
                    {

                        foreach (var userInfo in result.user_info_list)
                        {
                            string userId = null;
                            if (string.IsNullOrEmpty(userInfo.openid) || !openIdDic.ContainsKey(userInfo.openid))
                            {
                                //no openid or exist openid, move next
                                continue;
                            }
                            else
                            {
                                userId = openIdDic[userInfo.openid];
                                UpdateUserSnsInfo(db, userId, userInfo);
                            }
                        }
                        db.SaveChanges();
                        user100 = users.Skip(pageSize * page++).Take(pageSize).ToList();
                    }
                }


                return Content("OK");
            }
        }

        //update single claim type
        private bool UpdateUserSnsInfo(IdentityEntities db, string userId, IList<UserSnsInfo> currInfos, string claimType, string infoValue)
        {
            if (currInfos == null || string.IsNullOrEmpty(claimType) || string.IsNullOrEmpty(infoValue))
            {
                return false;
            }

            var currInfo = currInfos.FirstOrDefault(x => claimType.Equals(x.InfoKey));
            if (currInfo != null)
            {
                currInfo.InfoValue = infoValue;
            }
            else
            {
                db.UserSnsInfos.Add(new UserSnsInfo()
                {
                    UserId = userId,
                    SnsName = WeChatAuthenticationTypes.MP,
                    InfoKey = claimType,
                    InfoValue = infoValue
                });
            }

            return true;
        }
        private bool UpdateUserSnsInfo(IdentityEntities db, string userId, Senparc.Weixin.MP.AdvancedAPIs.User.UserInfoJson userInfo)
        {
            if (userInfo == null)
            {
                return false;
            }

            var currInfos = db.UserSnsInfos.Where(x => x.UserId == userId).ToList();


            //identify by unionid
            UpdateUserSnsInfo(db, userId, currInfos, System.Security.Claims.ClaimTypes.NameIdentifier, userInfo.unionid);

            //open id
            UpdateUserSnsInfo(db, userId, currInfos, WeChatClaimTypes.OpenId, userInfo.openid);

            //union id
            UpdateUserSnsInfo(db, userId, currInfos, WeChatClaimTypes.UnionId, userInfo.unionid);

            //nickname
            UpdateUserSnsInfo(db, userId, currInfos, WeChatClaimTypes.NickName, userInfo.nickname);

            //city
            UpdateUserSnsInfo(db, userId, currInfos, WeChatClaimTypes.City, userInfo.city);

            //province
            UpdateUserSnsInfo(db, userId, currInfos, WeChatClaimTypes.Province, userInfo.province);

            //country
            UpdateUserSnsInfo(db, userId, currInfos, WeChatClaimTypes.Country, userInfo.country);

            //headimage
            UpdateUserSnsInfo(db, userId, currInfos, WeChatClaimTypes.HeadImageUrl, userInfo.headimgurl);

            //sex
            UpdateUserSnsInfo(db, userId, currInfos, WeChatClaimTypes.Sex, userInfo.sex == 1 ? "M" : "F");

            //group
            UpdateUserSnsInfo(db, userId, currInfos, WeChatClaimTypes.GroupId, userInfo.groupid.ToString());

            //subscribed
            UpdateUserSnsInfo(db, userId, currInfos, WeChatClaimTypes.Subscribed, userInfo.subscribe == 0 ? "False" : "True");
            if (userInfo.subscribe == 1)
            {
                UpdateUserSnsInfo(db, userId, currInfos, WeChatClaimTypes.SubscribedTime, DateTime.Now.AddMilliseconds(-userInfo.subscribe_time).ToString());
            }

            //loaded time
            UpdateUserSnsInfo(db, userId, currInfos, WeChatClaimTypes.LoadedTime, DateTime.Now.ToString());

            return true;
        }
        #endregion
    }
}