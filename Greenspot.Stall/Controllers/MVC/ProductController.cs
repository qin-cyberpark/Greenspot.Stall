using Greenspot.Stall.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Greenspot.Stall.Controllers.MVC
{
    public class ProductController : Controller
    {
        private StallEntities _db = new StallEntities();

        // GET: Product
        public ActionResult Detail(string id)
        {
            ViewBag.Product = Product.FindById(id, _db);
            return View();
        }
    }
}