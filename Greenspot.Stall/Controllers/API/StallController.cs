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
        public OperationResult<IList<DeliveryOptionCollectionViewModel>> GetDeliveryOptions(int id, 
            [FromUri]string country, [FromUri]string city, [FromUri]string suburb, [FromUri]string area, [FromUri]decimal orderAmount)
        {
            string areaStr = $"{country}:{city}:{area.Replace(' ', '-')}".ToLower();
            var result = new OperationResult<IList<DeliveryOptionCollectionViewModel>>(true)
            {
                Data = new List<DeliveryOptionCollectionViewModel>()
            };

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
            var distance = stall.GetDistance(country, city, suburb);


            IList<DeliveryOrPickupOption> deliveryOpts;
            if (stall.Setting.Delivery.DeliveryType == Models.Settings.DeliveryTypes.StoreOnly)
            {
                //get store delivery
                deliveryOpts = stall.GetDeliveryOptions(DateTime.Now, advDays, areaStr, distance, orderAmount)
                    .OrderBy(x => x.From).ToList();
            }
            else
            {
                //get platform
                deliveryOpts = StallApplication.GetDeliveryOptions(stall, DateTime.Now, advDays, areaStr, distance, orderAmount)
                                    .OrderBy(x => x.From).ToList();
            }


            DeliveryOptionCollectionViewModel collection = null;
            DateTime currDate = DateTime.MinValue.Date;

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
                    if (!optObj.IsApplicableToArea(areaStr))
                    {
                        //has suitable area = available
                        continue;
                    }

                    if (opt.From.Date != currDate)
                    {
                        //new date group
                        collection = new DeliveryOptionCollectionViewModel();
                        result.Data.Add(collection);
                        currDate = opt.From.Date;
                    }

                    //add
                    collection.Options.Add(optObj);
                }
            }

            return result;
        }

        [HttpGet]
        [Route("api/stall/{id}/GetPickUpOptions")]
        public OperationResult<IList<PickupOptionGroupViewModel>> GetPickUpOptions(int id)
        {
            var result = new OperationResult<IList<PickupOptionGroupViewModel>>(true)
            {
                Data = new List<PickupOptionGroupViewModel>()
            };

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



            var advDays = stall.Setting.MaxAdvancedOrderDays > StallApplication.Setting.MaxAdvancedOrderDays ?
                            StallApplication.Setting.MaxAdvancedOrderDays : stall.Setting.MaxAdvancedOrderDays;

            var advMins = stall.Setting.MinPickupAdvancedMinutes < StallApplication.Setting.MinPickupAdvancedMinutes ?
                            StallApplication.Setting.MinPickupAdvancedMinutes : stall.Setting.MinPickupAdvancedMinutes;

            //get pickup delivery
            var pickupOptions = stall.GetPickupOptions(DateTime.Now.AddMinutes(advMins), advDays);

            PickupOptionGroupViewModel addrGrp = null;
            PickupOptionGroupViewModel dateGrp = null;
            string currAddr = null;
            DateTime currDate = DateTime.MinValue.Date;

            foreach (var opt in pickupOptions)
            {
                if (!opt.PickUpAddress.Equals(currAddr))
                {
                    //new address
                    addrGrp = new PickupOptionGroupViewModel()
                    {
                        Groups = new List<PickupOptionGroupViewModel>()
                    };
                    result.Data.Add(addrGrp);
                    currAddr = opt.PickUpAddress;
                    currDate = DateTime.MinValue.Date;
                }

                if (opt.From.Date != currDate)
                {
                    //new date group
                    dateGrp = new PickupOptionGroupViewModel();
                    addrGrp.Groups.Add(dateGrp);
                    currDate = opt.From.Date;
                }

                //add options
                if (opt.DivisionType == Models.Settings.TimeDivisionTypes.Undivisible)
                {
                    dateGrp.Options.Add(opt);
                }
                else
                {
                    ((List<DeliveryOrPickupOption>)dateGrp.Options).AddRange(opt.Divide());
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