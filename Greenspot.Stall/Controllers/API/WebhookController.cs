using Greenspot.SDK.Vend;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using Greenspot.Stall.Models;

namespace Greenspot.Stall.Controllers.API
{

    public class WebhookController : ApiController
    {
        private static readonly log4net.ILog _sysLogger = log4net.LogManager.GetLogger("SysLogger");
        private static readonly log4net.ILog _bizLogger = log4net.LogManager.GetLogger("BizLogger");

        [HttpPost]
        public async void ProductUpdate()
        {
            string data = null;
            try
            {
                data = await Request.Content.ReadAsStringAsync();
                var payload = data.Substring(data.IndexOf("&payload=") + 9);
                var jsonStr = System.Web.HttpUtility.UrlDecode(payload);

                //parse
                var vP = JsonConvert.DeserializeObject<VendProduct>(jsonStr);
                using (var db = new StallEntities())
                {
                    if (vP.DeletedAt != null)
                    {
                        //delete
                        Models.Product.DeleteById(vP.Id, db);
                    }
                    else
                    {
                        //new or update
                        var stall = Models.Stall.FindByRetailerIdAndSuppilerName(vP.RetailerId, vP.Supplier?.Name ?? vP.SupplierName, db);
                        if (stall != null)
                        {
                            await stall.UpdateProductById(vP.Id, db);
                        }
                    }
                }

                _sysLogger.Info(data);
            }
            catch (Exception ex)
            {
                _sysLogger.Info("failed to parse product update date", ex);
                _sysLogger.Info(string.Format("product update data: {0}", data));
            }
        }

        [HttpPost]
        public async void InventoryUpdate()
        {
            string data = null;
            try
            {

                data = await Request.Content.ReadAsStringAsync();
                var payload = data.Substring(data.IndexOf("&payload=") + 9);
                var jsonStr = System.Web.HttpUtility.UrlDecode(payload);

                //parse
                var vPI = JsonConvert.DeserializeObject<VendInventory>(jsonStr);
                using (var db = new StallEntities())
                {
                    Models.Product.SetInventoryById(vPI.ProductId, Convert.ToInt32(vPI.Count), db);
                }

                _sysLogger.Info(data);
            }
            catch (Exception ex)
            {
                _sysLogger.Info("failed to parse Inventory update date", ex);
                _sysLogger.Info(string.Format("Inventory update data: {0}", data));
            }
        }
    }
}
