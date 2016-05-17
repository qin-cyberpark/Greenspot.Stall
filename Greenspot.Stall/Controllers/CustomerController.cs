using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Greenspot.Stall.Controllers
{
    public class CustomerController : Controller
    {
        // GET: Account
        public ActionResult Orders()
        {
            return View();
        }
    }
}