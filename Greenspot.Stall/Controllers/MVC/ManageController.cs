using Greenspot.Stall.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;

namespace Greenspot.Stall.Controllers.MVC
{
    public class ManageController : Controller
    {
        private StallEntities _db = new StallEntities();

        // GET: Manage
        public ActionResult Stall(string id)
        {
            var stall = Models.Stall.FindByVendPrefix(id, _db);
            if (stall == null)
            {
                stall = new Models.Stall();
            }
            else
            {
                _db.Entry(stall).Collection(x => x.Webhooks).Load();
            }
            return View(stall);
        }

        public async Task<ActionResult> Init(string id)
        {
            var stall = Models.Stall.FindById(id, _db);
            if (stall != null && string.IsNullOrEmpty(stall.PaymentTypeId) && string.IsNullOrEmpty(stall.DeliveryProductId))
            {
                var result = await stall.Init();
                if (result.Succeeded)
                {
                    return Redirect("/manage/stall/" + stall.Prefix);
                }
                else
                {
                    return Content(result.Message);
                }
            }

            return Content("stall id=" + id + "不存在");
        }
    }
}