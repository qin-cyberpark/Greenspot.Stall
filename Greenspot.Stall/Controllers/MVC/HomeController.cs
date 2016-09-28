using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Greenspot.Stall.Models;

namespace Greenspot.Stall.Controllers
{
    public class HomeController : Controller
    {
        // GET: Default
        private StallEntities _db = new StallEntities();
        public ActionResult Index()
        {
            return Redirect("/index.html");
        }

        public ActionResult Search()
        {
            return View();
        }

        public ActionResult Homemade()
        {
            return Redirect("/homemade/index.html");
        }
    }
}