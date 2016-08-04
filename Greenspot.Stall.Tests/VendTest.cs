using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Greenspot.Stall.Models;
using System.Threading.Tasks;
using Greenspot.SDK.Vend;
namespace Greenspot.Stall.Tests
{
    [TestClass]
    public class VendTest
    {
        private StallEntities _db;

        [TestInitialize]
        public void InitTest()
        {
            _db = new StallEntities();
        }

        [TestMethod]
        public async Task CreateWebHookTest()
        {
            var req = new SDK.Vend.VendWebhook()
            {
                Type = SDK.Vend.VendWebhook.VendWebhookTypes.ProductUpdate,
                Url = "http://stall.greenspot.net.nz/api/webhook/ProductUpdate",
                Active = true
            };
            var result = await Greenspot.SDK.Vend.VendWebhook.CreateVendWebhookAsync(req, "qincyber", "5OtjwgBqfHJZgkkjCrzrP:hELHuupEA7GkGDlz9E");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetWebHookTest()
        {
            var result = await SDK.Vend.VendWebhook.GetWebhooksAsync("qincyber", "5OtjwgBqfHJZgkkjCrzrP:hELHuupEA7GkGDlz9E");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task DeleteWebHookTest()
        {
            var result = await SDK.Vend.VendWebhook.GetWebhooksAsync("qincyber", "5OtjwgBqfHJZgkkjCrzrP:fCnfop7yJDuFys9x52");
            if(result != null)
            {
                foreach(var hook in result)
                {
                    var status = await SDK.Vend.VendWebhook.DeleteWebhookAsync("qincyber", hook.Id, "5OtjwgBqfHJZgkkjCrzrP:fCnfop7yJDuFys9x52");
                }
            }
        }

        [TestMethod]
        public void DistanceTest()
        {
            var result = Greenspot.Stall.Utilities.DistanceMatrix.GetSuburbDistaince("NZ","Auckland", "New Windsor", "nz", "Auckland","Mt Raskial");
            Assert.IsNotNull(result);
        }
    }
}
