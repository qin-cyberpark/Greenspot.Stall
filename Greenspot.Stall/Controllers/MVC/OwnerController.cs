﻿using Greenspot.Identity;
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
            //
            var stalls = Models.Stall.FindByUserId(CurrentUser.Id, _db);
            if (stalls.Count == 0)
            {
                //have not registered
                return View("Apply");
            }

            ViewBag.Stalls = stalls;

            return View();
        }

        #region Register
        //Step 1
        public ActionResult Apply()
        {
            return View();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Apply(OwnerRegisterViewModel ownerInfo)
        {
            OperationResult<string> result = new OperationResult<string>(true);
            try
            {
                var user = Models.User.CurrentUser;
                //create stall
                var opt = Models.Stall.CraeteStall(user.Id, ownerInfo.StallName, ownerInfo.VendPrefix, _db);
                if (!opt.Succeeded)
                {
                    result.Message = opt.Message;
                    result.Succeeded = false;
                }
                else
                {
                    var stall = opt.Data;
                    stall.ContactName = ownerInfo.ContactName;
                    stall.Email = ownerInfo.Email;
                    stall.Mobile = ownerInfo.Mobile;
                    stall.Status = Models.Stall.StallStatus.Applied;
                    stall.Save();
                    //result.Data = StallApplication.GetAuthorisationCodeUri(ownerInfo.VendPrefix, user.Id);
                }
            }
            catch (Exception ex)
            {
                result.Message = "申请店铺失败";
                result.Exception = ex;
                result.Succeeded = false;
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
            if (!string.IsNullOrEmpty(accessToken))
            {
                return Content(string.Format("Vend {0} binding succeed", prefix));
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
        public async Task<ActionResult> InitStall(int id)
        {
            try
            {
                var stall = Models.Stall.FindById(id, _db);
                if (stall == null)
                {
                    StallApplication.BizErrorFormat("stall {0} not exist", id);
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
            }
            catch (Exception ex)
            {
                StallApplication.BizError("Failed to init stall", ex);
                return View("Error");
            }
        }
        #endregion

        #region Setting
        [Authorize]
        public ActionResult Setting(int id)
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