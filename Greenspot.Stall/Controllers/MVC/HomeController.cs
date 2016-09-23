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
            //ViewBag.Products = Product.GetHomepageProducts(_db);
            return Redirect("/index.html");
        }

        public ActionResult Takeaway()
        {
            return Redirect("/takeaway/index.html");
        }

        public ActionResult Homemade()
        {
            return Redirect("/homemade/index.html");
        }

        public ActionResult Search()
        {
            string category = Request["c"];
            string area = Request["a"];
            string keyword = Request["k"];

            ViewBag.MatchedStalls = Models.Stall.Search(category, area, keyword, _db);
            ViewBag.MatchedProducts = Product.Search(category, area, keyword, _db);

            return View();
        }
    }
}