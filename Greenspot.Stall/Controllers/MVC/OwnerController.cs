using Greenspot.Identity;
using Microsoft.AspNet.Identity.Owin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Greenspot.Identity.Extensions;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using Greenspot.SDK.Vend;
using Greenspot.Identity.OAuth.WeChat;
using Greenspot.Stall.Models;
using Greenspot.Stall.Models.ViewModels;

namespace Greenspot.Stall.Controllers.MVC
{
    public class OwnerController : Controller
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

        // GET: Owner
        [Authorize]
        public ActionResult Index()
        {

            if (!Models.User.CurrentUser.HasName)
            {
                //have not registered
                return Redirect("~/owner/register");
            }

            var stalls = Models.Stall.FindByUserId(CurrentUser.Id, _db);
            ViewBag.Stalls = stalls;

            return View();
        }
        #region Register
        //Step 1
        public ActionResult Register()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Register(OwnerRegisterViewModel ownerInfo)
        {
            var user = Models.User.CurrentUser;
            user.Email = ownerInfo.Email;
            user.PhoneNumber = ownerInfo.Mobile;
            user.FirstName = ownerInfo.FirstName;
            user.LastName = ownerInfo.LastName;
            OperationResult<string> result = new OperationResult<string>(true);
            if (!user.Save(_db))
            {
                result.Message = "无法保存商户信息";
                result.Succeeded = false;
            }
            else
            {
                //create stall
                var opt = Models.Stall.CraeteStall(user.Id, ownerInfo.StallName, ownerInfo.VendPrefix, _db);
                if (!opt.Succeeded)
                {
                    result.Message = opt.Message;
                    result.Succeeded = false;
                }
                else
                {
                    result.Data = StallApplication.GetAuthorisationCodeUri(ownerInfo.VendPrefix, user.Id);
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        //vend call back
        public async Task<ActionResult> Connect()
        {
            var code = Request["code"];
            var prefix = Request["domain_prefix"];
            var userId = Request["state"];
            var accessToken = await StallApplication.GetAccessTokenAsync(prefix, code);
            if (!string.IsNullOrEmpty(accessToken) && CurrentUser.Id.Equals(userId))
            {
                return RedirectToAction("InitStall", new { id = prefix });
            }
            else
            {
                return View("Error");
            };
        }

        public ActionResult InitStall(string id)
        {
            ViewBag.Prefix = id;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> InitStall()
        {
            try
            {
                var prefex = Request["prefix"];
                var stall = Models.Stall.FindByVendPrefix(prefex, _db);
                if (stall == null)
                {
                    StallApplication.BizErrorFormat("stall {0} not exist", prefex);
                    return View("Error");
                }

                if (!stall.UserId.Equals(CurrentUser.Id))
                {
                    StallApplication.BizErrorFormat("user id not match {0} <> {1}", stall.UserId, CurrentUser.Id);
                    return View("Error");
                }

                var initResult = await stall.Init();
                if (initResult.Succeeded)
                {
                    return RedirectToAction("Index");
                }
                else
                {
                    StallApplication.BizErrorFormat("Failed to init stall, {0}", initResult.Message);
                    return View("Error");
                }
            }catch(Exception ex){
                StallApplication.BizError("Failed to init stall", ex);
                return View("Error");
            }
        }
        #endregion

        #region Setting
        [Authorize]
        public ActionResult Setting(string id)
        {
            var user = Models.User.CurrentUser;
            var stall = Models.Stall.FindById(id, _db);
            if (!stall.UserId.Equals(CurrentUser.Id))
            {
                StallApplication.SysErrorFormat("user id not match {0} <> {1}", stall.UserId, CurrentUser.Id);
                return View("Error");
            }

            ViewBag.Stall = stall;

            return View();
        }
        #endregion
    }
}