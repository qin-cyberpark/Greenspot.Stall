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

namespace Greenspot.Stall.Controllers
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
                if (_userManager == null) {
                    _userManager = HttpContext.GetOwinContext().GetUserManager<GreenspotUserManager>();
                }
                return _userManager;
            }
        }

        // GET: Owner
        [Authorize]
        public ActionResult Index()
        {
            if(string.IsNullOrEmpty(CurrentUser.Email) || string.IsNullOrEmpty(CurrentUser.PhoneNumber))
            {
                //have not registered
                return RedirectToAction("Register");
            }

            var stalls = Models.Stall.FindByUserId(CurrentUser.Id, _db);
            if(stalls == null || stalls.Count == 0)
            {
                //no stall
            }

            ViewBag.Stalls = stalls;
         
            return View();
        }

        public ActionResult Register()
        {
            return View();
        }

        public async Task<ActionResult> Connect()
        {
            var code = Request["code"];
            var prefix = Request["domain_prefix"];
            var state = Request["state"];
            var accessToken = await StallApplication.GetAccessTokenAsync(prefix, code);
            ViewBag.Connected = !string.IsNullOrEmpty(accessToken);
            return View();
        }
    }
}