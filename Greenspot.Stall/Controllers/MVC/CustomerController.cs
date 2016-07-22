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
using Greenspot.Configuration;

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

            catch
            {
                // Try and handle malformed POST body
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Session["CURRENT_ORDER"] = order;
            OperationResult<string> result = new OperationResult<string>(true);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        [HttpPost]
        public ActionResult AddAddress()
        {
            Stream req = Request.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            string json = new StreamReader(req).ReadToEnd();

            DeliveryAddress addr = null;
            try
            {
                addr = JsonConvert.DeserializeObject<DeliveryAddress>(json);
                var suburb = Suburb.Find(addr.Suburb, addr.City, "NZ", _db);
                addr.CountryId = "NZ";
                addr.Area = suburb.Area;
                addr.UserId = CurrentUser.Id;
                addr.Save(_db);
            }

            catch
            {
                // Try and handle malformed POST body
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            OperationResult<bool> result = new OperationResult<bool>(true);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        [HttpGet]
        public ActionResult Checkout()
        {
            ViewBag.Order = (OrderViewModel)Session["CURRENT_ORDER"];
            return View();
        }

        [Authorize]
        public ActionResult Addresses()
        {
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

        private Order ConvertToOrder(string jsonString)
        {
            var orderVM = JsonConvert.DeserializeObject<OrderViewModel>(jsonString);
            var order = new Order();
            order.StallId = orderVM.Stall.Id;
            order.UserId = CurrentUser.Id;

            //product
            foreach (var i in orderVM.Stall.Items)
            {
                var product = Product.FindById(i.Id, _db);
                if (product.Stock < i.Quantity)
                {
                    //check stock
                }
                order.Items.Add(new OrderItem()
                {
                    Product = product,
                    Quantity = i.Quantity
                });
            }

            //delivery
            var stall = Models.Stall.FindById(orderVM.Stall.Id, _db);
            var deliveryProduct = Product.FindById(stall.DeliveryProductId, _db);
            deliveryProduct.DisplayRetailPriceTaxInclusive = orderVM.DeliveryFee;
            if (orderVM.DeliveryOption.IsPickUp)
            {
                var devAddr = DeliveryAddress.FindById(int.Parse(orderVM.DeliveryAddress.Id), _db);
                deliveryProduct.LineNote = orderVM.DeliveryOption.ToString() + " " + devAddr.ToString();

            }
            else
            {
                deliveryProduct.LineNote = orderVM.DeliveryOption.ToString();
                deliveryProduct.DisplayRetailPriceTaxInclusive = orderVM.DeliveryFee;
            }


            order.Items.Add(new OrderItem()
            {
                Product = deliveryProduct,
                Quantity = 1
            });

            order.Note = orderVM.Note + "\n" + deliveryProduct.LineNote;

            //discount
            return order;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult> Pay()
        {
            OperationResult<string> result = new OperationResult<string>(true);

            Stream req = Request.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            string json = new StreamReader(req).ReadToEnd();

            try
            {
                var order = ConvertToOrder(json);

                //create order
                if (!order.Create(_db))
                {
                    result.Succeeded = false;
                    result.Message = "fail to create order";
                }

                //return payment url
                string payUrl = null;
                string urlSuccess = string.Format("{0}/Customer/PxPay/{1}?paid=SUCCESS", GreenspotConfiguration.RootUrl, order.Id);
                string urlFail = string.Format("{0}/Customer/PxPay/{1}?paid=FAIL", GreenspotConfiguration.RootUrl, order.Id);

                try
                {
                    payUrl = Accountant.GeneratePayURL(order, urlFail, urlSuccess);
                    StallApplication.BizInfoFormat("[PAY]go to payment url:{0}", payUrl);
                    result.Data = payUrl;
                }
                catch (Exception ex)
                {
                    StallApplication.SysError("[PAY]", ex);
                    result.Succeeded = false;
                    result.Message = "fail to generate payment url";
                }
            }
            catch (Exception ex)
            {
                // Try and handle malformed POST body
                StallApplication.SysError("[PAY]", ex);
                result.Succeeded = false;
                result.Message = "fail to pay";
            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [Authorize(Roles ="Admin")]
        [HttpPost]
        public async Task<ActionResult> FakePay()
        {
            OperationResult<string> result = new OperationResult<string>(true);

            Stream req = Request.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            string json = new StreamReader(req).ReadToEnd();

            try
            {
                //save to vend
                var order = ConvertToOrder(json);

                //create order
                if (order.Create(_db))
                {
                    await order.Save(_db);
                }
            }
            catch
            {

            }

            return Redirect("/customer/orders");
        }
        public async Task<ActionResult> PxPay(int id)
        {
            string paidFlag = Request["paid"];
            if (paidFlag == null)
            {
                StallApplication.SysError("[MSG]pxpay callback without paid parameter");
                return Redirect("~/ErrorPage");
            }
            StallApplication.SysInfoFormat("[MSG]PxPay call back [{0}]:{1}", paidFlag, Request.Url.ToString());
            bool isSuccess = "SUCCESS".Equals(paidFlag.ToUpper());

            string payResultId = Request["result"];
            if (string.IsNullOrEmpty(payResultId))
            {
                StallApplication.SysError("[MSG]pxpay callback without result id");
                return Redirect("~/ErrorPage");
            }

            int paidOrderId = 0;
            if (!Accountant.VerifyPxPayPayment(payResultId, isSuccess, out paidOrderId))
            {
                StallApplication.BizErrorFormat("[MSG]PxPay not verified, result id={0}", payResultId);
                return Redirect("~/ErrorPage");
            }

            if (paidOrderId != id)
            {
                StallApplication.BizErrorFormat("[MSG]transaction not matched, px {0} <> url {1}", paidOrderId, id);
                return Redirect("~/ErrorPage");
            }

            if (isSuccess)
            {
                //save to vend
                var order = Order.FindById(id, _db);
                await order.Save(_db);

                //send message
                try
                {
                    var owner = UserManager.FindById(order.Stall.UserId);
                    var openId = owner?.SnsInfos[WeChatClaimTypes.OpenId].InfoValue;
                    if (!string.IsNullOrEmpty(openId))
                    {
                        var msg = string.Format("店铺[{0}]有一个新订单 [{1}]", order.Stall.Name, order.Id);
                        WeChatHelper.SendMessage(openId, msg);
                    }
                }
                catch
                {

                }

                ////clear cookie
                //if (Request.Cookies["cart"] != null)
                //{
                //    HttpCookie myCookie = new HttpCookie("cart");
                //    myCookie.Expires = DateTime.Now.AddDays(-1d);
                //    Response.Cookies.Add(myCookie);
                //}

                return Redirect("/customer/orders");
            }
            else
            {
                return Redirect("~/ErrorPage");
            }
        }
    }
}