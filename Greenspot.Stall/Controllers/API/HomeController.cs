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


    public class HomeController : ApiController
    {
        private StallEntities _db = new StallEntities();

        // GET api/<controller>
        [HttpGet]
        public OperationResult<IList<StallViewModel>> Recommend([FromUri(Name = "c")]string category, [FromUri(Name = "a")]string area)
        {
            var result = new OperationResult<IList<StallViewModel>>(true);
            var oriStalls = Models.Stall.GetRecommend(category, area, _db, 5);
            IList<StallViewModel> stalls = new List<StallViewModel>();
            foreach (var s in oriStalls)
            {
                var stallVm = new StallViewModel
                {
                    Id = s.Id,
                    Name = s.StallName,
                    Products = new List<StallProductViewModel>()
                };

                var products = s.SellingProducts.Take(3);
                foreach (var p in products)
                {
                    stallVm.Products.Add(new StallProductViewModel()
                    {
                        Id = p.Id,
                        Name = p.Name
                    });
                }
                stalls.Add(stallVm);
            }

            result.Data = stalls;

            return result;
        }

        // GET api/<controller>
        [HttpGet]
        public OperationResult<IList<StallViewModel>> SearchStall([FromUri(Name = "c")]string category, [FromUri(Name = "a")]string area, [FromUri(Name = "k")]string keyword
                                                                    , [FromUri(Name = "p")]int page = 0, [FromUri(Name = "ps")]int pageSize = 10)
        {
            var result = new OperationResult<IList<StallViewModel>>(true);
            var oriStalls = Models.Stall.Search(_db, category, area, keyword, page, pageSize);
            IList<StallViewModel> stalls = new List<StallViewModel>();
            foreach (var s in oriStalls)
            {
                var stallVm = new StallViewModel
                {
                    Id = s.Id,
                    Name = s.StallName,
                    Products = new List<StallProductViewModel>()
                };

                var products = s.SellingProducts.Take(3);
                foreach (var p in products)
                {
                    stallVm.Products.Add(new StallProductViewModel()
                    {
                        Id = p.Id,
                        Name = p.Name
                    });
                }
                stalls.Add(stallVm);
            }

            result.Data = stalls;

            return result;
        }

        // GET api/<controller>
        [HttpGet]
        public OperationResult<IList<StallProductViewModel>> SearchProduct([FromUri(Name = "c")]string category, [FromUri(Name = "a")]string area, [FromUri(Name = "k")]string keyword,
                                                                            [FromUri(Name = "p")]int page = 0, [FromUri(Name = "ps")]int pageSize = 10)
        {
            var result = new OperationResult<IList<StallProductViewModel>>(true);
            var oriProducts = Product.Search(_db, category, area, keyword, page, pageSize);
            IList<StallProductViewModel> products = new List<StallProductViewModel>();
            foreach (var p in oriProducts)
            {
                if (p.Price != null)
                {
                    products.Add(new StallProductViewModel()
                    {
                        Id = p.Id,
                        Name = p.BaseName,
                        Image = p.Image,
                        Price = p.PriceIncTax,
                        StallId = p.StallId,
                        StallName = p.Stall.StallName
                    });
                }
            }

            result.Data = products;
            return result;
        }
    }
}
