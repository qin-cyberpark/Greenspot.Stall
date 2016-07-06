using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Greenspot.Stall.Models;
using Greenspot.Stall.Models.ViewModels;
using Greenspot.SDK.Vend;
using System.Threading.Tasks;
using Greenspot.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.AspNet.Identity;
using Greenspot.Identity.OAuth.WeChat;
using Greenspot.Stall.Utilities;
using System.IO;
using System.Net;

namespace Greenspot.Stall.Controllers.MVC
{
    public class CustomerController : Controller
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

        // GET: Account
        [Authorize]
        public ActionResult Orders()
        {
            ViewBag.Orders = Order.FindByUserId(CurrentUser.Id, _db);
            return View();
        }

        [Authorize]
        public ActionResult Cart()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult CheckStock()
        {
            Stream req = Request.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            string json = new StreamReader(req).ReadToEnd();

            OrderViewModel order = null;
            try
            {
                order = JsonConvert.DeserializeObject<OrderViewModel>(json);
            }

            catch (Exception ex)
            {
                // Try and handle malformed POST body
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Session["CURRENT_ORDER"] = order;
            OperationResult<string> result = new OperationResult<string>(true);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        [HttpGet]
        public ActionResult Checkout()
        {
            ViewBag.Order = (OrderViewModel)Session["CURRENT_ORDER"];
            Session.Remove("CURRENT_ORDER");
            return View();
        }

        [Authorize]
        [HttpGet]
        public ActionResult DeliveryAddresses()
        {
            _db.Configuration.LazyLoadingEnabled = false;
            _db.Configuration.ProxyCreationEnabled = false;
            var addresses = DeliveryAddress.FindByUserId(CurrentUser.Id, _db);
            OperationResult<DeliveryAddress[]> result = new OperationResult<DeliveryAddress[]>(true);
            if (addresses == null)
            {
                result.Succeeded = false;
            }
            else
            {
                result.Data = addresses.ToArray();
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Pay()
        {
            Stream req = Request.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            string json = new StreamReader(req).ReadToEnd();

            OrderViewModel order = null;
            try
            {
                order = JsonConvert.DeserializeObject<OrderViewModel>(json);
            }

            catch (Exception ex)
             {
                // Try and handle malformed POST body
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //var orderRawData = System.Web.HttpUtility.UrlDecode(Request.Cookies["cart"].Value);

            ////create order object
            //var cart = JsonConvert.DeserializeObject<CartViewModel>(orderRawData);
            //var stall = Models.Stall.FindById(cart.CurrentStall.Id, _db);

            ////separate stalls
            //var order = new Order();
            //order.Id = Guid.NewGuid().ToString();
            //order.Stall = stall;
            //order.UserId = CurrentUser.Id;
            //foreach (var i in cart.CurrentStall.Items)
            //{
            //    var product = Product.FindById(i.Id, _db);
            //    order.Items.Add(new OrderItem()
            //    {
            //        Product = product,
            //        Quantity = i.Quantity
            //    });
            //}

            ////
            //await order.Save(_db);

            ////send message
            //try
            //{
            //    var owner = UserManager.FindById(order.Stall.UserId);
            //    var openId = owner?.SnsInfos[WeChatClaimTypes.OpenId].InfoValue;
            //    if (!string.IsNullOrEmpty(openId))
            //    {
            //        var msg = string.Format("店铺[{0}]有一个新订单，金额[{1:C}]", order.Stall.Name, order.Total);
            //        WeChatHelper.SendMessage(openId, msg);
            //    }
            //}
            //catch
            //{

            //}

            ////clear cookie
            //if (Request.Cookies["cart"] != null)
            //{
            //    HttpCookie myCookie = new HttpCookie("cart");
            //    myCookie.Expires = DateTime.Now.AddDays(-1d);
            //    Response.Cookies.Add(myCookie);
            //}

            return RedirectToAction("Orders");
        }
    }
}