using Greenspot.Stall.Models;
using Greenspot.Stall.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Greenspot.Stall.Controllers.API
{
    public class StallController : ApiController
    {
        private static readonly log4net.ILog _sysLogger = log4net.LogManager.GetLogger("SysLogger");
        private static readonly log4net.ILog _bizLogger = log4net.LogManager.GetLogger("BizLogger");

        [HttpGet]
        public OperationResult<DeliveryTimeViewModel[]> DeliverySchedule(string id, [FromUri]string country, [FromUri]string city, [FromUri]string area)
        {
            var result = new OperationResult<DeliveryTimeViewModel[]>(true);

            Models.Stall stall = null;
            //get stall 
            using (var db = new StallEntities())
            {
                stall = Models.Stall.FindById(id, db);
                if(stall == null)
                {
                    result.Succeeded = false;
                    result.Message = "Can not load delivery schedule for stall " + id;
                    return result;
                }
            }

            //get next 7 days delivery schedule
            var scheduleItems = stall.GetSchedule(country, city, area, 3);
            var deliveryOptions = new List<DeliveryTimeViewModel>();
            foreach(var item in scheduleItems)
            {
                deliveryOptions.Add(new DeliveryTimeViewModel()
                {
                    From = item.From,
                    To = item.To,
                    IsPickUp = item.IsPickUp,
                    Area = item.AreaString,
                    PickUpAddress = item.PickUpAddress
                });
            }
            result.Data = deliveryOptions.ToArray();

            return result;
        }

        [HttpGet]
        public OperationResult<decimal> CalcDeliveryFee(string id, [FromUri]string country, [FromUri]string city, [FromUri]string suburb, [FromUri]decimal amount)
        {
            var result = new OperationResult<decimal>(true);

            Models.Stall stall = null;
            //get stall 
            using (var db = new StallEntities())
            {
                stall = Models.Stall.FindById(id, db);
                if (stall == null)
                {
                    result.Succeeded = false;
                    result.Message = "Can not load delivery schedule for stall " + id;
                    return result;
                }
            }

            //get next 7 days delivery schedule
            var fee = stall.GetDeliveryFee(country, city, suburb, amount);
            if (fee == null)
            {
                result.Succeeded = false;
                result.Message = "Can not get delivery fee for stall " + id;
            }
            else
            {
                result.Data = fee.Value;
            }

            return result;
        }
    }
}
