using Greenspot.Identity;
using Greenspot.Identity.OAuth.WeChat;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Greenspot.Stall.Utilities;
using Microsoft.AspNet.Identity;
using System.Threading.Tasks;
using System.Web.Hosting;
using Greenspot.Stall.Models;

namespace Greenspot.Stall.Controllers.MVC
{
    public class TestController : Controller
    {
        private StallEntities _db = new StallEntities();
        private GreenspotUserManager _userManager;
        private GreenspotUser _currentUser;
        public GreenspotUser CurrentUser
        {
            get
            {
                if (_currentUser == null)
                {
                    _currentUser = UserManager.FindByIdAsync(User.Identity.GetUserId()).Result;
                }
                return _currentUser;
            }
        }
        public GreenspotUserManager UserManager
        {

            get
            {
                if (_userManager == null)
                {
                    _userManager = HttpContext.GetOwinContext().GetUserManager<GreenspotUserManager>();
                }
                return _userManager;
            }
        }

        // GET: Test
        [Authorize]
        public ActionResult MsgMe()
        {
            //notify
            var openId = CurrentUser.SnsInfos[WeChatClaimTypes.OpenId].InfoValue;
            HostingEnvironment.QueueBackgroundWorkItem(x => WeChatHelper.SendMessageAsync(openId, "this is a test message"));
            return Content($"test message was sent to {openId}");
        }

        [Authorize]
        public ActionResult OrderNotify(int id)
        {
            var stall = Models.Stall.FindById(id, _db);
            if (stall == null)
            {
                return Content($"store does not exist");
            }

            if (!stall.UserId.Equals(CurrentUser.Id))
            {
                return Content($"no permission to test the store");
            }

            var order = new Order()
            {
                Id = 999999,
                StallAmount = 10.99M,
                StallDiscount = 0.0M,
                PlatformDiscount = 0.0M,
                PlatformDelivery = 5.99M,
                ChargeAmount = 16.98M,
                CreateTime = DateTime.Now,
                DeliveryTimeStart = DateTime.Now,
                DeliveryTimeEnd = DateTime.Now,
                Receiver = "Contact,021-123456",
                DeliveryAddress = "999 Test Road, Auckland",
                Note = "this is a test order"
            };

            order.Stall = stall;

            order.Items.Add(new OrderItem() { Name = "Product 1", Price = 4.0M, Quantity = 1 });
            order.Items.Add(new OrderItem() { Name = "Product 2", Price = 3.0M, Quantity = 1 });
            order.Items.Add(new OrderItem() { Name = "Product 3", Price = 3.99M, Quantity = 1 });

            //notify
            var openIds = new List<string>();
            //owner
            var owner = UserManager.FindById(order.Stall.UserId);
            var ownerId = owner?.SnsInfos[WeChatClaimTypes.OpenId].InfoValue;
            openIds.Add(ownerId);

            //delivery man
            var deliveryMen = Models.User.GetByRole(_db, "DeliveryMan");
            foreach (var d in deliveryMen)
            {
                var dId = d.SnsInfos.FirstOrDefault(x => WeChatClaimTypes.OpenId.Equals(x.InfoKey))?.InfoValue;
                if (!string.IsNullOrEmpty(dId))
                {
                    openIds.Add(dId);
                }
            }

            HostingEnvironment.QueueBackgroundWorkItem(x => order.SendToWechat(openIds));

            var str = string.Join(",", openIds);

            return Content($"test order notification was sent to {str}");
        }
    }
}