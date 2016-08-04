using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Greenspot.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;
using Microsoft.AspNet.Identity;
using Greenspot.Identity.OAuth.WeChat;
using Greenspot.Configuration;
using Greenspot.Stall.Models;
using Greenspot.Stall.Models.ViewModels;

namespace Greenspot.Stall.Controllers.MVC
{
    public class AccountController : Controller
    {
        private GreenspotSignInManager _signInManager;
        private GreenspotUserManager _userManager;

        public AccountController()
        {
        }

        public AccountController(GreenspotUserManager userManager, GreenspotSignInManager signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public GreenspotSignInManager SignInManager
        {
            get
            {
                return _signInManager ?? HttpContext.GetOwinContext().Get<GreenspotSignInManager>();
            }
            private set
            {
                _signInManager = value;
            }
        }

        public GreenspotUserManager UserManager
        {
            get
            {
                return _userManager ?? HttpContext.GetOwinContext().GetUserManager<GreenspotUserManager>();
            }
            private set
            {
                _userManager = value;
            }
        }

        // GET: Auth
        public ActionResult Index()
        {
            Session["code"] = Request["code"];
            Session["state"] = Request["state"];
            return RedirectToAction("Orders", "Customer");
        }

        //[AllowAnonymous]
        //public ActionResult Login(string returnUrl)
        //{
        //    // Request a redirect to the external login provider
        //    return new ChallengeResult(WeChatAuthenticationTypes.MP, Url.Action("WeChatMpLoginCallback", "Account", new { ReturnUrl = returnUrl }));
        //}

        // POST: /Account/WeChatMpLogin
        [AllowAnonymous]
        public ActionResult WeChatMpLogin(string returnUrl)
        {
#if !DEBUG
            // Request a redirect to the external login provider
            return new ChallengeResult(WeChatAuthenticationTypes.MP, Url.Action("WeChatMpLoginCallback", "Account", new { ReturnUrl = returnUrl }));
#else
            //UserManager.AddPassword("f2c0021f-5165-439d-b5e4-72a61be7aed7", "testtest");
            var stattus = SignInManager.PasswordSignIn("test", "testtest", false, false);
            return RedirectToLocal(returnUrl);
#endif
        }

        [AllowAnonymous]
        public async Task<ActionResult> WeChatMpLoginCallback(string returnUrl)
        {
            var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (loginInfo == null)
            {
                StallApplication.BizErrorFormat("[MSG]failed to get login info");
                return Redirect("~/ErrorPage");
            }

            // Sign in the user with this external login provider if the user already has a login
            var result = await SignInManager.ExternalSignInAsync(loginInfo, isPersistent: false);
            switch (result)
            {
                case SignInStatus.Success:
                    return RedirectToLocal(returnUrl);
                case SignInStatus.LockedOut:
                    return View("Lockout");
                case SignInStatus.RequiresVerification:
                    return RedirectToAction("SendCode", new { ReturnUrl = returnUrl, RememberMe = false });
                case SignInStatus.Failure:
                default:
                    // If the user does not have an account, then prompt the user to create an account
                    ViewBag.ReturnUrl = returnUrl;
                    ViewBag.LoginProvider = loginInfo.Login.LoginProvider;

                    GreenspotUser user = new GreenspotUser() { UserName = "" };
                    var rslt = await UserManager.CreateAsync(loginInfo, user);
                    if (rslt.Succeeded)
                    {
                        await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);
                        return RedirectToLocal(returnUrl);
                    }
                    return RedirectToLocal(returnUrl);
            }
        }

        #region Helpers
        // Used for XSRF protection when adding external logins
        private const string XsrfKey = "XsrfId";

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error);
            }
        }

        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        internal class ChallengeResult : HttpUnauthorizedResult
        {
            public ChallengeResult(string provider, string redirectUri)
                : this(provider, redirectUri, null)
            {
            }

            public ChallengeResult(string provider, string redirectUri, string userId)
            {
                LoginProvider = provider;
                RedirectUri = redirectUri;
                UserId = userId;
            }

            public string LoginProvider { get; set; }
            public string RedirectUri { get; set; }
            public string UserId { get; set; }

            public override void ExecuteResult(ControllerContext context)
            {
                var properties = new AuthenticationProperties { RedirectUri = RedirectUri };
                if (UserId != null)
                {
                    properties.Dictionary[XsrfKey] = UserId;
                }
                context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
            }
        }
        #endregion

    }
}