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
            ViewBag.Products = Product.GetHomepageProducts(_db);
            return View();
        }

        public ActionResult Takeway()
        {
            return View();
        }

        public ActionResult Preorder()
        {
            return View();
        }

        public ActionResult Search()
        {
            var keyword = Request["keyword"];
            var category = Request["category"];
            var area = Request["area"];
            if (string.IsNullOrEmpty(keyword))
            {
                return RedirectToAction("Index");
            }

            ViewBag.Products = Product.Search(keyword, _db);
            ViewBag.Stalls = Models.Stall.Search(keyword, _db);
            return View();
        }
    }
}