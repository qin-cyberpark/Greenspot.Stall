using System;
using System.IO;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Greenspot.Stall.Models;

namespace Greenspot.Stall.Storefont.Tests
{
    [TestClass]
    public class DeliveryFeeTest
    {
        private StallEntities _db;

        [TestInitialize]
        public void InitTest()
        {
            _db = new StallEntities();
        }

        private string LoadJson(string fileName)
        {
            using (var file = new StreamReader(fileName))
            {
                return file.ReadToEnd();
            }
        }

        [TestMethod]
        public void TestPickupOptions()
        {
            var json = LoadJson(@"D:\Works\Greenspot\Greenspot.Stall\Greenspot.Stall.Tests\data\store_setting_1.json");
            var stall = new Models.Stall();
            stall.SettingJson = json;

            var pickup = stall.GetPickupOptions(new DateTime(2016, 10, 20), 6);
            Assert.AreEqual(11, pickup.Count);
            Assert.IsTrue(pickup[0].PickUpAddress.StartsWith("1103"));
            Assert.IsTrue(pickup[6].PickUpAddress.StartsWith("ABC"));
            Assert.AreEqual(new DateTime(2016, 10, 20, 17, 0, 0), pickup[0].To);
            Assert.AreEqual(new DateTime(2016, 10, 21, 10, 0, 0), pickup[1].From);
            Assert.AreEqual(new DateTime(2016, 10, 22, 22, 0, 0), pickup[2].To);
            Assert.AreEqual(new DateTime(2016, 10, 24, 10, 0, 0), pickup[3].From);
            Assert.AreEqual(new DateTime(2016, 10, 26, 22, 0, 0), pickup[5].To);
            Assert.AreEqual(new DateTime(2016, 10, 20, 14, 0, 0), pickup[6].From);
            Assert.AreEqual(new DateTime(2016, 10, 26, 18, 0, 0), pickup[10].To);
        }


        [TestMethod]
        public void TestStoreDeliveryOptions()
        {
            var json = LoadJson(@"D:\Works\Greenspot\Greenspot.Stall\Greenspot.Stall.Tests\data\store_setting_2.json");
            var stall = new Models.Stall();
            stall.SettingJson = json;

            var delivery = stall.GetDeliveryOptions(new DateTime(2016, 10, 20, 16, 30, 0), 7, "nz:auckland:auckland:auckland-city", null, null);
            Assert.AreEqual(7, delivery.Count);
        }

        [TestMethod]
        public void TestPlatformDeliveryOptions()
        {
            var json = LoadJson(@"D:\Works\Greenspot\Greenspot.Stall\Greenspot.Stall.Tests\platform.setting.json");
            var stall = new Models.Stall();
            stall.SettingJson = json;

            var delivery = StallApplication.GetDeliveryOptions(stall, new DateTime(2016, 10, 20, 16, 30, 0), 7,
                "nz:auckland:auckland:auckland-city", null, null);
            Assert.AreEqual(3, delivery.Count);
        }
    }
}
