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
    public class ProductController : ApiController
    {
        private StallEntities _db = new StallEntities();
        private static readonly log4net.ILog _sysLogger = log4net.LogManager.GetLogger("SysLogger");
        private static readonly log4net.ILog _bizLogger = log4net.LogManager.GetLogger("BizLogger");


        [HttpGet]
        [Route("api/product/search")]
        public OperationResult<IList<StallProductViewModel>> Search([FromUri(Name = "c")]string category, [FromUri(Name = "a")]string area, [FromUri(Name = "k")]string keyword,
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
                        StallName = p.Stall.StallName,
                        Description = p.Description,
                        Stock = p.Stock,
                        TrackInventory = p.TrackInventory
                    });
                }
            }

            result.Data = products;
            return result;
        }

        [HttpGet]
        [Route("api/product/{productId}")]
        public OperationResult<StallProductViewModel> GetProductById(string productId)
        {
            var result = new OperationResult<StallProductViewModel>(true);

            //get stall 
            StallProductViewModel productVM = null;
            Product product = null;
            using (var db = new StallEntities())
            {
                product = Product.FindById(productId, db);
                if (product == null)
                {
                    result.Succeeded = false;
                    result.Message = "Can not load products " + productId;
                    return result;
                }

                productVM = new StallProductViewModel()
                {
                    Id = product.Id,
                    Name = product.BaseName,
                    Image = product.ImageLarge,
                    Price = product.PriceIncTax,
                    StallId = product.StallId,
                    StallName = product.Stall.StallName,
                    Description = product.Description,
                    Stock = product.Stock,
                    TrackInventory = product.TrackInventory
                };
            }

            result.Data = productVM;
            return result;
        }
    }
}