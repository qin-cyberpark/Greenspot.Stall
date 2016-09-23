using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Greenspot.Stall.Models;
using Greenspot.Stall.Utilities;
using Senparc.Weixin.MP.Containers;
using Greenspot.Configuration;
namespace Greenspot.Stall.Storefont.Tests
{
    [TestClass]
    public class OrderTest
    {
        private StallEntities _db;

        [TestInitialize]
        public void InitTest()
        {
            _db = new StallEntities();
            AccessTokenContainer.Register(GreenspotConfiguration.AccessAccounts["wechat"].Id, GreenspotConfiguration.AccessAccounts["wechat"].Secret);
        }

        [TestMethod]
        public void OrderNotifier()
        {
            //load order
            var order = Order.FindById(8, _db);
            order.Notify(_db, "opDxls3kxQNdVPqkKW4c8DAfDGX8").Wait();
        }


        [TestMethod]
        public async void PrintOrder()
        {
            //load order
            var order = Order.FindById(7, _db);
            var result = await PrintHelper.PrintOrderAsync(order);
        }

        [TestMethod]
        public async void PrintSampleOrder()
        {
            var order = new Order()
            {
                Id = 999999,
                StallAmount = 999.99M,
                StallDiscount = 999.99M,
                PlatformDiscount = 999.99M,
                PlatformDelivery = 999.99M,
                ChargeAmount = 999.99M,
                CreateTime = DateTime.Now,
                DeliveryTimeStart = DateTime.Now,
                DeliveryTimeEnd = DateTime.Now,
                Receiver = "Qin XXX,0211231234",
                DeliveryAddress = "Level 5, 396 Queen St, Auckland",
                Note = "一A二B三C四D五E六F七G八H九I十J,一A二B三C四D五E六F七G八H九I十J,一A二B三C四D五E六F七G八H九I十J一A二B三C四D五E六F七G八H九I十J"
            };

            order.Stall = new Models.Stall
            {
                Id = 1,
                StallName = "测试小店",
                PrinterAddress = "67bfaa0f72c03202",
                IsPrintOrder = true
            };

            order.Items.Add(new OrderItem() { Name = "一A二B三C四D五E一A二B", Price = 20.01M, Quantity = 10 });
            order.Items.Add(new OrderItem() { Name = "一A二B", Price = 20.01M, Quantity = 10 });
            order.Items.Add(new OrderItem() { Name = "一A二B三C四D五E一A二B三C四D五E一A二B三C四D五EA二B三C四D五E", Price = 101.01M, Quantity = 9 });

            var result = await PrintHelper.PrintOrderAsync(order);
        }
    }
}
