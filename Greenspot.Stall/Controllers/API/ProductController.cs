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
        private static readonly log4net.ILog _sysLogger = log4net.LogManager.GetLogger("SysLogger");
        private static readonly log4net.ILog _bizLogger = log4net.LogManager.GetLogger("BizLogger");


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