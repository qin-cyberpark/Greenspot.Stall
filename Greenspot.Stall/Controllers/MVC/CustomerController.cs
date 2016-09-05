﻿using Newtonsoft.Json;
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
        private static object _locker = new object();
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
            ViewBag.Orders = Models.Order.FindByUserId(CurrentUser.Id, _db).ToList();
            return View();
        }

        [Authorize]
        public ActionResult Order(int id)
        {
            var order = Models.Order.FindById(id, _db);
            if (order != null && CurrentUser.Id.Equals(order.UserId))
            {
                ViewBag.Order = order;
            }
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

            OrderCollectionViewModel orders = null;
            try
            {
                orders = JsonConvert.DeserializeObject<OrderCollectionViewModel>(json);
            }

            catch
            {
                // Try and handle malformed POST body
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            Session["CURRENT_ORDERS"] = orders;
            OperationResult<string> result = new OperationResult<string>(true);
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        [HttpPost]
        public ActionResult AddAddress()
        {
            OperationResult<bool> result = new OperationResult<bool>(true);

            Stream req = Request.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            string json = new StreamReader(req).ReadToEnd();

            DeliveryAddress addr = new DeliveryAddress();
            try
            {
                var clntObj = JsonConvert.DeserializeObject<dynamic>(json);
                addr.UserId = CurrentUser.Id;
                addr.Name = clntObj.FullName;
                addr.Mobile = clntObj.Mobile;
                addr.Address1 = clntObj.AddressObject.AddressLine1;
                addr.Address2 = clntObj.Address2;
                addr.Suburb = clntObj.AddressObject.Suburb;
                addr.City = clntObj.AddressObject.CityTown;
                addr.State = "";
                addr.CountryId = "NZ";
                addr.Postcode = clntObj.AddressObject.Postcode;
                addr.FullAddress = (string.IsNullOrEmpty(addr.Address2) ? "" : addr.Address2 + ", ") + clntObj.AddressObject.FullAddress;
                addr.Area = clntObj.Area;

                addr.Save(_db);
            }
            catch (Exception ex)
            {
                // Try and handle malformed POST body
                result.Succeeded = false;
                result.Message = ex.Message;
                StallApplication.SysError("[ADD ADDR]failed to address", ex);

            }

            return Json(result, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        [HttpGet]
        public ActionResult Checkout()
        {
            var orderCollection = (OrderCollectionViewModel)Session["CURRENT_ORDERS"];
            if (orderCollection == null)
            {
                return View("Cart");
            }
            var stockMsgs = new List<string>();

            //check stock
            foreach (var order in orderCollection.Orders)
            {
                foreach (var item in order.Items)
                {
                    var pdt = Product.FindById(item.Id, _db);
                    if (pdt.Stock < item.Quantity)
                    {
                        stockMsgs.Add(string.Format("{0}仅剩{1}件(份)", pdt.Name, pdt.Stock));
                    }
                }
            }
            if (stockMsgs.Count > 0)
            {
                ViewBag.StockMsgs = stockMsgs;
                return View("Cart");
            }
            else
            {
                ViewBag.Orders = orderCollection;
                return View();
            }
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
            //var orderVM = JsonConvert.DeserializeObject<OrderViewModel>(jsonString);
            //var order = new Order();
            //order.StallId = orderVM.Stall.Id;
            //order.UserId = CurrentUser.Id;

            ////product
            //foreach (var i in orderVM.Stall.Items)
            //{
            //    var product = Product.FindById(i.Id, _db);
            //    if (product.Stock < i.Quantity)
            //    {
            //        //check stock
            //    }
            //    order.Items.Add(new OrderItem(product)
            //    {
            //        Quantity = i.Quantity
            //    });
            //}

            ////delivery
            //var stall = Models.Stall.FindById(orderVM.Stall.Id, _db);
            //var deliveryProduct = Product.FindById(stall.DeliveryProductId, _db);

            //string addrStr = "";
            //if (!orderVM.DeliveryOption.IsPickUp)
            //{
            //    var devAddr = DeliveryAddress.FindById(int.Parse(orderVM.DeliveryAddress.Id), _db);
            //    addrStr = devAddr.ToString();
            //}
            //deliveryProduct.LineNote = orderVM.DeliveryOption.ToString() + "\n" + addrStr;
            //deliveryProduct.Price = orderVM.DeliveryOption.Fee;

            //order.Items.Add(new OrderItem(deliveryProduct)
            //{
            //    Quantity = 1
            //});

            //order.Note = orderVM.Note + "\n" + deliveryProduct.LineNote;

            ////discount
            //return order;
            return null;
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

        [Authorize(Roles = "Admin")]
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
            catch (Exception ex)
            {
                StallApplication.SysError("[MSG]fake pay failed", ex);
            }

            return Redirect("/customer/orders");
        }
        public async Task<ActionResult> PxPay(int id)
        {
            string paidFlag = Request["paid"];
            if (paidFlag == null)
            {
                StallApplication.SysError("[MSG]pxpay callback without paid parameter");
                return View("Error");
            }

            StallApplication.SysInfoFormat("[MSG]PxPay call back [{0}]:{1}", paidFlag, Request.Url.ToString());
            bool isSuccess = "SUCCESS".Equals(paidFlag.ToUpper());

            string payResultId = Request["result"];
            if (string.IsNullOrEmpty(payResultId))
            {
                StallApplication.SysError("[MSG]pxpay callback without result id");
                return View("Error");
            }

            int paidOrderId = 0;
            if (!Accountant.VerifyPxPayPayment(payResultId, isSuccess, out paidOrderId))
            {
                StallApplication.BizErrorFormat("[MSG]PxPay not verified, result id={0}", payResultId);
                return View("Error");
            }

            if (paidOrderId != id)
            {
                StallApplication.BizErrorFormat("[MSG]transaction not matched, px {0} <> url {1}", paidOrderId, id);
                return View("Error");
            }

            if (isSuccess)
            {
                //if (StallApplication.IsOrderOperating(id))
                //{
                //    StallApplication.BizErrorFormat("[MSG]order {0} is operating", id);
                //    return Redirect("/customer/orders");
                //}


                //save to vend

                Models.Order order = null;

                //save order
                lock (_locker)
                {
                    order = Models.Order.FindById(id, _db);

                    if (!string.IsNullOrEmpty(order.Status))
                    {
                        StallApplication.BizErrorFormat("[MSG]vend sale for order {0} is exist", id);
                        return Redirect("/customer/orders");
                    }

                    order.Status = "OPERATED";
                    _db.SaveChanges();
                }

                try
                {
                    //send message
                    var owner = UserManager.FindById(order.Stall.UserId);
                    var openId = owner?.SnsInfos[WeChatClaimTypes.OpenId].InfoValue;
                    if (!string.IsNullOrEmpty(openId))
                    {
                        var msg = string.Format("店铺[{0}]有一个新订单\r{1}", order.Stall.StallName, order.Summary);
                        WeChatHelper.SendMessage(openId, msg);
                    }
                }
                catch (Exception ex)
                {
                    StallApplication.SysError("[MSG]failed to send message", ex);
                }

                try
                {
                    await order.Save(_db);
                }
                catch (Exception ex)
                {
                    StallApplication.SysError("[MSG]failed to save order", ex);
                }

                //StallApplication.RemoveOperatingOrder(order.Id);

                return Redirect("/customer/orders?remove_stallId=" + order.StallId);
            }
            else
            {
                return View("Error");
            }
        }
    }
}