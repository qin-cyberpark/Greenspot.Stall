using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Greenspot.Stall.Controllers
{
    public class StallController : Controller
    {
        // GET: Storefront
        public ActionResult Index()
        {
            return View();
        }
    }
}