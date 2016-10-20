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
        private StallEntities _db = new StallEntities();
        private static readonly log4net.ILog _sysLogger = log4net.LogManager.GetLogger("SysLogger");
        private static readonly log4net.ILog _bizLogger = log4net.LogManager.GetLogger("BizLogger");

        // GET api/<controller>
        [HttpGet]
        [Route("api/stall/recommend")]
        public OperationResult<IList<StallViewModel>> Recommend([FromUri(Name = "c")]string category, [FromUri(Name = "a")]string area)
        {
            var result = new OperationResult<IList<StallViewModel>>(true);
            var oriStalls = Models.Stall.GetRecommend(category, area, _db, 20);
            IList<StallViewModel> stalls = new List<StallViewModel>();
            foreach (var s in oriStalls)
            {
                var stallVm = new StallViewModel
                {
                    Id = s.Id,
                    Name = s.StallName,
                    InitialProducts = new List<StallProductViewModel>()
                };

                var products = s.SellingProducts.Take(3);
                foreach (var p in products)
                {
                    stallVm.InitialProducts.Add(new StallProductViewModel()
                    {
                        Id = p.Id,
                        Name = p.BaseName
                    });
                }
                stalls.Add(stallVm);
            }

            result.Data = stalls;

            return result;
        }

        [HttpGet]
        [Route("api/stall/search")]
        public OperationResult<IList<StallViewModel>> Search([FromUri(Name = "c")]string category, [FromUri(Name = "a")]string area, [FromUri(Name = "k")]string keyword,
                                                            [FromUri(Name = "p")]int page = 0, [FromUri(Name = "ps")]int pageSize = 10)
        {
            var result = new OperationResult<IList<StallViewModel>>(true);
            var oriStalls = Models.Stall.Search(_db, category, area, keyword, page, pageSize);
            IList<StallViewModel> stalls = new List<StallViewModel>();
            foreach (var s in oriStalls)
            {
                var stallVm = new StallViewModel
                {
                    Id = s.Id,
                    Name = s.StallName
                };
                stalls.Add(stallVm);
            }

            result.Data = stalls;

            return result;
        }

        [HttpGet]
        [Route("api/stall/{id}/GetDeliveryOptions")]
        public OperationResult<IList<DeliveryOptionCollectionViewModel>> GetDeliveryOptions(int id, [FromUri]string area, [FromUri]decimal orderAmount)
        {
            string areaStr = $"nz:auckland:auckland:{area.ToLower().Replace(' ', '-')}";
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

            //minimum delivery order amount
            if (stall.Setting.Delivery.MinOrderAmount != null &&
                orderAmount < stall.Setting.Delivery.MinOrderAmount)
            {
                return result;
            }

            //
            var advDays = stall.Setting.MaxAdvancedOrderDays > StallApplication.Setting.MaxAdvancedOrderDays ?
                                      StallApplication.Setting.MaxAdvancedOrderDays : stall.Setting.MaxAdvancedOrderDays;

            var advMins = stall.Setting.MinPickupAdvancedMinutes < StallApplication.Setting.MinPickupAdvancedMinutes ?
                            StallApplication.Setting.MinPickupAdvancedMinutes : stall.Setting.MinPickupAdvancedMinutes;


            //get distance
            //var distance = stall.GetDistance(country, city, suburb);

            //
            var collections = new SortedList<DateTime, DeliveryOptionCollectionViewModel>();

            IList<DeliveryOrPickupOption> deliveryOpts;
            if (stall.Setting.Delivery.DeliveryType == Models.Settings.DeliveryTypes.StoreOnly)
            {
                //get store delivery
                deliveryOpts = stall.GetDeliveryOptions(DateTime.Now.AddMinutes(advMins), advDays, areaStr, null, orderAmount)
                    .OrderBy(x => x.From).ToList();
            }
            else
            {
                //get platform
                deliveryOpts = StallApplication.GetDeliveryOptions(stall, DateTime.Now.AddMinutes(advMins), advDays, areaStr, null, orderAmount)
                                    .OrderBy(x => x.From).ToList();
            }


            DeliveryOptionCollectionViewModel collection = null;
            IList<DeliveryOrPickupOption> destList = null;

            foreach (var opt in deliveryOpts)
            {
                //free delivery 
                if (stall.Setting.Delivery.FreeDeliveryOrderAmount != null &&
                    orderAmount >= stall.Setting.Delivery.FreeDeliveryOrderAmount)
                {
                    opt.Fee = 0;
                }

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
                    ApplicableOptions = c.ApplicableOptions.OrderBy(x => x.Fee).OrderBy(x => x.From).ToList(),
                    OtherOptions = c.OtherOptions.OrderBy(x => x.Fee).OrderBy(x => x.From).ToList()
                };

                if (n.ApplicableOptions.Count > 0)
                {
                    result.Data.Add(n);
                }
            }

            return result;

        }

        [HttpGet]
        [Route("api/stall/{id}/GetPickUpOptions")]
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

            //var earliestOrderTime = DateTime.Now.AddMinutes(stall.DeliveryPlan.MinOrderAcvancedMinutes);

            //
            var collections = new SortedList<DateTime, DeliveryOptionCollectionViewModel>();

            var advDays = stall.Setting.MaxAdvancedOrderDays > StallApplication.Setting.MaxAdvancedOrderDays ?
                            StallApplication.Setting.MaxAdvancedOrderDays : stall.Setting.MaxAdvancedOrderDays;

            var advMins = stall.Setting.MinPickupAdvancedMinutes < StallApplication.Setting.MinPickupAdvancedMinutes ?
                            StallApplication.Setting.MinPickupAdvancedMinutes : stall.Setting.MinPickupAdvancedMinutes;

            //get pickup delivery
            var pickupOptions = stall.GetPickupOptions(DateTime.Now.AddMinutes(advMins), advDays).OrderBy(x => x.From).ToList();

            DeliveryOptionCollectionViewModel collection = null;

            foreach (var opt in pickupOptions)
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
                if (opt.DivisionType == Models.Settings.TimeDivisionTypes.Undivisible)
                {
                    collection.ApplicableOptions.Add(opt);
                }
                else
                {
                    ((List<DeliveryOrPickupOption>)collection.ApplicableOptions).AddRange(opt.Divide());
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
                    ApplicableOptions = c.ApplicableOptions.OrderBy(x => x.From).OrderBy(x => x.Fee).ToList()
                };

                if (n.ApplicableOptions.Count > 0)
                {
                    result.Data.Add(n);
                }
            }

            return result;

        }

        [HttpGet]
        [Route("api/stall/{id}")]
        public OperationResult<StallViewModel> Get(int id)
        {
            var result = new OperationResult<StallViewModel>(true);

            //get stall 
            StallViewModel stallVM = null;
            Models.Stall stall = null;
            using (var db = new StallEntities())
            {
                stall = Models.Stall.FindById(id, db);
                if (stall == null)
                {
                    result.Succeeded = false;
                    result.Message = "Can not load products for stall " + id;
                    return result;
                }

                stallVM = new StallViewModel()
                {
                    Id = stall.Id,
                    Name = stall.StallName,
                    Categories = stall.Categories,
                    ShowCategory = stall.ShowCategory,
                    InitialProducts = new List<StallProductViewModel>()
                };
            }

            //initial products
            var initProducts = stall.SellingProducts;
            if (stall.ShowCategory)
            {
                //recommend
                initProducts = initProducts.Take(stall.RecommendNumber).ToList();
            }

            foreach (var p in initProducts)
            {
                if (p.Active == true && p.Stock > 0 && p.Price != null)
                {
                    stallVM.InitialProducts.Add(new StallProductViewModel()
                    {
                        Id = p.Id,
                        Name = p.BaseName,
                        Image = p.Image,
                        Price = p.PriceIncTax,
                        StallId = p.StallId,
                        StallName = p.Stall.StallName,
                        Description = p.Description,
                        Stock = p.Stock,
                        TrackInventory = p.TrackInventory
                    });
                }
            }

            result.Data = stallVM;
            return result;
        }

        [HttpGet]
        [Route("api/stall/{id}/category/{cateIdx?}")]
        public OperationResult<IList<StallProductViewModel>> ProductsByCategory(int id, int cateIdx = 0)
        {
            var result = new OperationResult<IList<StallProductViewModel>>(true)
            {
                Data = new List<StallProductViewModel>()
            };

            //get stall 
            Models.Stall stall = null;
            using (var db = new StallEntities())
            {
                stall = Models.Stall.FindById(id, db);
                if (stall == null)
                {
                    result.Succeeded = false;
                    result.Message = "Can not load products for stall " + id;
                    return result;
                }
            }



            IList<Product> products;
            if (Category.Recommend.Index == cateIdx)
            {
                products = stall.SellingProducts.Take(stall.RecommendNumber).ToList();
            }
            else
            {
                string category = null;
                if (Category.Others.Index == cateIdx)
                {
                    category = Category.Others.Identifier;
                }
                else
                {
                    foreach (var c in stall.Categories)
                    {
                        if (c.Index == cateIdx)
                        {
                            category = c.Identifier;
                            break;
                        }
                    }
                }

                products = stall.GetProductsByCategory(category);
            }
            foreach (var p in products)
            {
                if (p.Active == true && p.Stock > 0 && p.Price != null)
                {
                    result.Data.Add(new StallProductViewModel()
                    {
                        Id = p.Id,
                        Name = p.BaseName,
                        Image = p.Image,
                        Price = p.PriceIncTax,
                        StallId = p.StallId,
                        StallName = p.Stall.StallName,
                        Description = p.Description,
                        Stock = p.Stock,
                        TrackInventory = p.TrackInventory
                    });
                }
            }

            return result;
        }
    }
}