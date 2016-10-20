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
using System.Web.Hosting;
using System.Text;

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
            ViewBag.Orders = Models.Order.FindByUserId(CurrentUser.Id, _db).Where(x => x.HasPaid).ToList();
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
                addr.StateOrRegion = "";
                addr.CountryCode = "NZ";
                addr.Postcode = clntObj.AddressObject.Postcode;
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
            //var orderCollection = (OrderCollectionViewModel)Session["CURRENT_ORDERS"];
            //if (orderCollection == null)
            //{
            //    return View("Cart");
            //}
            //var stockMsgs = new List<string>();

            ////check stock
            //foreach (var order in orderCollection.Orders)
            //{
            //    foreach (var item in order.Items)
            //    {
            //        var pdt = Product.FindById(item.Id, _db);
            //        if (pdt.Stock < item.Quantity)
            //        {
            //            stockMsgs.Add(string.Format("{0}仅剩{1}件(份)", pdt.Name, pdt.Stock));
            //        }
            //    }
            //}
            //if (stockMsgs.Count > 0)
            //{
            //    ViewBag.StockMsgs = stockMsgs;
            //    return View("Cart");
            //}
            //else
            //{
            //    ViewBag.Orders = orderCollection;
            //    return View();
            //}
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

        private IList<Order> ConvertToOrders(string jsonString, out IList<Product> outOtStocks)
        {
            var orders = new List<Order>();
            outOtStocks = new List<Product>();
            var orderVMs = JsonConvert.DeserializeObject<IList<OrderViewModel>>(jsonString);
            foreach (var orderVM in orderVMs)
            {
                var order = new Order();
                order.StallId = orderVM.StallId;
                order.StallName = orderVM.StallName;
                order.Stall = Models.Stall.FindById(orderVM.StallId, _db);
                order.UserId = CurrentUser.Id;

                //product
                foreach (var i in orderVM.Items)
                {
                    var product = Product.FindById(i.Id, _db);
                    if (product.Stock < i.Quantity)
                    {
                        //check stock
                        outOtStocks.Add(product);
                    }
                    order.Items.Add(new OrderItem(product)
                    {
                        Quantity = i.Quantity
                    });
                }

                //delivery
                order.DeliveryTimeStart = orderVM.DeliveryOption.From;
                order.DeliveryTimeEnd = orderVM.DeliveryOption.To;
                if (!orderVM.DeliveryOption.IsPickUp)
                {
                    var devAddr = DeliveryAddress.FindByCode(CurrentUser.Id, orderVM.DeliveryAddress.Code, _db);
                    order.Receiver = string.Format("{0} {1}", devAddr.Name, devAddr.Mobile);
                    order.DeliveryAddress = devAddr.FullAddress;
                }
                else
                {
                    order.Receiver = "PICK-UP";
                    order.DeliveryAddress = orderVM.DeliveryOption.PickUpAddress;
                }

                if (order.Stall.HasDelivery)
                {
                    //owner delivery
                    var deliveryProduct = Product.FindById(order.Stall.DeliveryProductId, _db);

                    deliveryProduct.LineNote = orderVM.DeliveryOption.ToString() + "\n" + order.Receiver + "\n" + order.DeliveryAddress;
                    deliveryProduct.Price = orderVM.DeliveryOption.Fee;

                    order.Items.Add(new OrderItem(deliveryProduct)
                    {
                        Quantity = 1
                    });
                }
                else
                {
                    //platform delivery
                    order.PlatformDelivery = orderVM.DeliveryOption.Fee;
                }

                //note
                order.Note = orderVM.Note;

                //amount
                order.StallAmount = order.CalcTotal();

                //discount
                order.StallDiscount = 0;
                order.PlatformDiscount = 0;

                //charge amount
                order.ChargeAmount = order.CalcTotalCharge();

                orders.Add(order);
            }


            return orders;
        }

        [Authorize]
        [HttpPost]
        public ActionResult Pay()
        {
            OperationResult<string> result = new OperationResult<string>(true);

            Stream req = Request.InputStream;
            req.Seek(0, System.IO.SeekOrigin.Begin);
            string json = new StreamReader(req).ReadToEnd();

            Payment payment = null;

            try
            {
                IList<Product> outOfStocks;
                var orders = ConvertToOrders(json, out outOfStocks);
                if (outOfStocks.Count > 0)
                {
                    result.Succeeded = false;
                    result.Message = "Some Products are out of stock";
                    var sb = new StringBuilder("[");
                    var comma = "";
                    foreach (var p in outOfStocks)
                    {
                        sb.Append(string.Format("{0}{{\"StallName\":\"{1}\",\"ProductName\":\"{2}\",\"Stock\":{3}}}", comma, p.Stall.StallName, p.BaseName, p.Stock));
                        comma = ",";
                    }
                    sb.Append("]");
                    result.ErrorType = "OutOfStock";
                    result.Data = sb.ToString();
                    return Json(result, JsonRequestBehavior.AllowGet);
                }

                using (var tran = _db.Database.BeginTransaction())
                {
                    //calc total amnount, orderIds
                    decimal amount = 0.0M;
                    foreach (var order in orders)
                    {
                        amount += order.ChargeAmount;
                    }

                    //create payment
                    payment = Payment.CreatePayment(_db, amount);
                    if (payment == null)
                    {
                        tran.Rollback();
                        result.Succeeded = false;
                        result.Message = "fail to create payment";
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }

                    //create order
                    string orderIds = null;
                    foreach (var order in orders)
                    {
                        order.PaymentId = payment.Id;
                        order.IsPrintOrder = order.Stall.IsPrintOrder;
                        if (!order.Save(_db))
                        {
                            tran.Rollback();
                            result.Succeeded = false;
                            result.Message = "fail to create order";
                            return Json(result, JsonRequestBehavior.AllowGet);
                        }

                        //get order id
                        if (string.IsNullOrEmpty(orderIds))
                        {
                            orderIds = order.Id.ToString();
                        }
                        else
                        {
                            orderIds += "," + order.Id.ToString();
                        }
                    }

                    //update payment
                    payment.OrderIds = orderIds;
                    payment.Save(_db);

                    //save
                    tran.Commit();
                }

                //return payment url
                string payUrl = null;
                string urlSuccess = string.Format("{0}/Customer/PxPay/{1}?paid=SUCCESS", GreenspotConfiguration.RootUrl, payment.Id);
                string urlFail = string.Format("{0}/Customer/PxPay/{1}?paid=FAIL", GreenspotConfiguration.RootUrl, payment.Id);

                try
                {
                    payUrl = Accountant.GeneratePayURL(payment, urlFail, urlSuccess);
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

        public ActionResult PxPay(int id)
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

            int paymentId = 0;
            if (!Accountant.VerifyPxPayPayment(payResultId, isSuccess, out paymentId))
            {
                StallApplication.BizErrorFormat("[MSG]PxPay not verified, result id={0}", payResultId);
                return View("Error");
            }

            if (paymentId != id)
            {
                StallApplication.BizErrorFormat("[MSG]transaction not matched, px {0} <> url {1}", paymentId, id);
                return View("Error");
            }

            if (isSuccess)
            {
                if (StallApplication.IsPaymentOperating(paymentId))
                {
                    StallApplication.BizErrorFormat("[MSG]payment {0} is operating", paymentId);
                    return Redirect("/customer/orders");
                }

                //set order as paid
                var orders = Models.Order.FindByPaymentId(paymentId, _db);
                foreach (var order in orders)
                {
                    if (!order.HasPaid)
                    {
                        try
                        {
                            order.HasPaid = true;
                            _db.SaveChanges();

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

                            //await order.Notify(_db, openId);
                            HostingEnvironment.QueueBackgroundWorkItem(x => order.Notify(_db, openIds));
                        }
                        catch (Exception ex)
                        {
                            StallApplication.SysError("[MSG]failed to save orders", ex);
                        }
                    }
                }

                StallApplication.RemoveOperatingPayment(paymentId);
                return Redirect("/customer/orders?act=paid");
            }
            else
            {
                return Redirect("/errorpage/payfailed");
            }
        }
    }
}