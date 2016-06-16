using Greenspot.Stall.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Greenspot.Stall.Controllers.MVC
{
    public class StallController : Controller
    {
        private StallEntities _db = new StallEntities();

        // GET: Stall
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Detail(string id)
        {
            ViewBag.Stall = Models.Stall.FindById(id, _db);
            return View();
        }
    }
}