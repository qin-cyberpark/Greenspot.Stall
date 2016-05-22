using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Greenspot.Stall.Controllers
{
    public class OwnerController : Controller
    {
        // GET: Owner
        [Authorize]
        public ActionResult Index()
        {
            return View();
        }
    }
}