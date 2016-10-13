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
        public async Task GetProduct()
        {
            var result = await SDK.Vend.VendProduct.GetProductsAsync("comingsoon", "5OtjwgBqfHJZhKmH2RSHV:jpTHSuyAXPj80ikLUc");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetProductById()
        {
            var result = await SDK.Vend.VendProduct.GetProductByIdAsync("02dcd191-ae7c-11e6-f485-4cfa0ba671ba", "qincyber", "5OtjwgBqfHJZgkkjCrzrP:OCubTLPAgHRHivZ6Yw");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task CreateWebHookTest()
        {
            var req = new SDK.Vend.VendWebhook()
            {
                Type = SDK.Vend.VendWebhook.VendWebhookTypes.ProductUpdate,
                Url = "http://demo.stall.greenspot.net.nz/api/webhook/ProductUpdate",
                Active = true
            };
            var result = await Greenspot.SDK.Vend.VendWebhook.CreateVendWebhookAsync(req, "qincyber", "5OtjwgBqfHJZgkkjCrzrP:OCubTLPAgHRHivZ6Yw");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task GetWebHookTest()
        {
            var result = await SDK.Vend.VendWebhook.GetWebhooksAsync("qincyber", "5OtjwgBqfHJZgkkjCrzrP:OCubTLPAgHRHivZ6Yw");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public async Task DeleteWebHookTest()
        {
            var result = await SDK.Vend.VendWebhook.GetWebhooksAsync("qincyber", "5OtjwgBqfHJZgkkjCrzrP:OCubTLPAgHRHivZ6Yw");
            if (result != null)
            {
                foreach (var hook in result)
                {
                    var status = await SDK.Vend.VendWebhook.DeleteWebhookAsync("qincyber", hook.Id, "5OtjwgBqfHJZgkkjCrzrP:OCubTLPAgHRHivZ6Yw");
                }
            }
        }

        [TestMethod]
        public async Task GetSupplierTest()
        {
            var result = await SDK.Vend.Supplier.GetSuppliersAsync("qincyber", "5OtjwgBqfHJZgkkjCrzrP:OCubTLPAgHRHivZ6Yw");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void DistanceTest()
        {
            var result = SuburbDistance.GetDistance("NZ", "Auckland", "New Windsor", "nz", "Auckland", "Mt Roskil");
            Assert.IsNotNull(result);
        }

        [TestMethod]
        public void FindByRetailerIdAndSuppilerNameTest()
        {
            var stall = Models.Stall.FindByRetailerIdAndSuppilerName("02dcd191-ae2b-11e6-f485-17ec0e9829db", "代理店铺2", _db);
            Assert.AreEqual(114, stall.Id);

            stall = Models.Stall.FindByRetailerIdAndSuppilerName("02dcd191-ae2b-11e6-f485-17ec0e9829db", null, _db);
            Assert.AreEqual(109, stall.Id);

            stall = Models.Stall.FindByRetailerIdAndSuppilerName("02dcd191-ae2b-11e6-f485-17ec0e9829db", "NOT_EXIST", _db);
            Assert.AreEqual(109, stall.Id);
        }
    }
}
