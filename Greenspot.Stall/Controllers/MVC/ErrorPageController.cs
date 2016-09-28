using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Greenspot.Stall.Models.ViewModels;
namespace Greenspot.Stall.Controllers.MVC
{
    public class ErrorPageController : Controller
    {
        // GET: ErrorPage
        public ActionResult Index()
        {
            ErrorViewModel model = new ErrorViewModel
            {
                Message = "error"
            };
            return View("error", model);
        }

        public ActionResult FileNotFound()
        {
            ErrorViewModel model = new ErrorViewModel
            {
                Type = ErrorViewModel.ErrorType.FileNotFound
            };
            return View("error", model);
        }
        public ActionResult UnauthorizedAccess()
        {
            ErrorViewModel model = new ErrorViewModel
            {
                Type = ErrorViewModel.ErrorType.UnauthorizedAccess
            };
            return View("error", model);
        }

        public ActionResult PayFailed()
        {
            ErrorViewModel model = new ErrorViewModel
            {
                Type = ErrorViewModel.ErrorType.PayFailed
            };
            return View("error", model);
        }
    }
}