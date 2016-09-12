using Greenspot.Stall.Models;
using Greenspot.Stall.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
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
        public OperationResult<IList<DeliveryOptionCollectionViewModel>> GetDeliveryOptions(int id,
            [FromUri]string country, [FromUri]string city, [FromUri]string suburb, [FromUri]string area)
        {
            var areaStr = string.Format("{0}-{1}-{2}", country, city, area);
            var result = new OperationResult<IList<DeliveryOptionCollectionViewModel>>(true);
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

            //
            var earliestOrderTime = DateTime.Now.AddMinutes(stall.DefaultOrderAdvancedMinutes == null ? 180 : stall.DefaultOrderAdvancedMinutes.Value);


            //get distance
            var distance = stall.GetDistance(country, city, suburb);

            //
            var collections = new SortedList<DateTime, DeliveryOptionCollectionViewModel>();

            //get temporary delivery
            var tempOpts = stall.DeliveryPlan.GetTemporaryDeliveryOptions(country, city, area,
                distance, DateTime.Now).OrderBy(x => x.From).ToList();

            #region temporary options
            DeliveryOptionCollectionViewModel collection = null;
            IList<DeliveryOption> destList = null;

            foreach (var opt in tempOpts)
            {
                var optObjs = opt.Divide();

                foreach (var optObj in optObjs)
                {
                    //got collection by date
                    if (collections.ContainsKey(optObj.From.Date))
                    {
                        collection = collections[optObj.From.Date];
                    }
                    else
                    {
                        collection = new DeliveryOptionCollectionViewModel()
                        {
                            Date = optObj.From.Date
                        };
                        collections[optObj.From.Date] = collection;
                    }

                    //destination list
                    if (optObj.IsApplicableToArea(areaStr))
                    {
                        //has suitable area = available
                        destList = collection.ApplicableOptions;
                    }
                    else
                    {
                        //other
                        destList = collection.OtherOptions;
                    }

                    //add
                    destList.Add(optObj);
                }
            }
            #endregion

            #region default options
            //get default delivery
            var dftOpts = stall.DeliveryPlan.GetDefaultDeliveryOptions(country, city, area,
                distance, DateTime.Now).OrderBy(x => x.From).ToList();

            foreach (var opt in dftOpts)
            {
                //got collection by date
                if (collections.ContainsKey(opt.From.Date))
                {
                    collection = collections[opt.From.Date];
                }
                else
                {
                    collection = new DeliveryOptionCollectionViewModel()
                    {
                        Date = opt.From.Date
                    };
                    collections[opt.From.Date] = collection;
                }

                if (opt.IsApplicableToArea(areaStr))
                {
                    //has suitable area = available
                    if (!opt.IsTimeDivisible)
                    {
                        collection.ApplicableOptions.Add(opt);
                    }
                    else
                    {
                        ((List<DeliveryOption>)collection.ApplicableOptions).AddRange(opt.Divide());
                    }
                }
            }
            #endregion

            //resort result
            result.Data = new List<DeliveryOptionCollectionViewModel>();
            foreach (var dt in collections.Keys)
            {
                var c = collections[dt];
                if (c.ApplicableOptions.Count == 0)
                {
                    continue;
                }
                var n = new DeliveryOptionCollectionViewModel()
                {
                    Date = dt,
                    ApplicableOptions = c.ApplicableOptions.Where(x => x.To > earliestOrderTime).OrderBy(x => x.Fee).OrderBy(x => x.From).ToList(),
                    OtherOptions = c.OtherOptions.Where(x => x.To > earliestOrderTime).OrderBy(x => x.Fee).OrderBy(x => x.From).ToList()
                };

                if (n.ApplicableOptions.Count > 0)
                {
                    result.Data.Add(n);
                }
            }

            return result;

        }

        [HttpGet]
        public OperationResult<IList<DeliveryOptionCollectionViewModel>> GetPickUpOptions(int id)
        {
            var result = new OperationResult<IList<DeliveryOptionCollectionViewModel>>(true);

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

            var earliestOrderTime = DateTime.Now.AddMinutes(stall.DefaultOrderAdvancedMinutes == null ? 180 : stall.DefaultOrderAdvancedMinutes.Value);

            //
            var collections = new SortedList<DateTime, DeliveryOptionCollectionViewModel>();

            //get temporary delivery
            var tempOpts = stall.DeliveryPlan.GetPickUpOptions(DateTime.Now).OrderBy(x => x.From).ToList();

            DeliveryOptionCollectionViewModel collection = null;

            foreach (var opt in tempOpts)
            {
                //got collection by date
                if (collections.ContainsKey(opt.From.Date))
                {
                    collection = collections[opt.From.Date];
                }
                else
                {
                    collection = new DeliveryOptionCollectionViewModel()
                    {
                        Date = opt.From.Date
                    };
                    collections[opt.From.Date] = collection;
                }

                //add
                if (!opt.IsTimeDivisible)
                {
                    collection.ApplicableOptions.Add(opt);
                }
                else
                {
                    ((List<DeliveryOption>)collection.ApplicableOptions).AddRange(opt.Divide());
                }
            }

            //resort result
            result.Data = new List<DeliveryOptionCollectionViewModel>();
            foreach (var dt in collections.Keys)
            {
                var c = collections[dt];
                if (c.ApplicableOptions.Count == 0)
                {
                    continue;
                }

                var n = new DeliveryOptionCollectionViewModel()
                {
                    Date = dt,
                    ApplicableOptions = c.ApplicableOptions.Where(x => x.To > earliestOrderTime).OrderBy(x => x.From).OrderBy(x => x.Fee).ToList()
                };

                if (n.ApplicableOptions.Count > 0)
                {
                    result.Data.Add(n);
                }
            }

            return result;

        }
    }
}