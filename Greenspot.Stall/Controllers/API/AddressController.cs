using Greenspot.Stall.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace Greenspot.Stall.Controllers.API
{
    public class AddressController : ApiController
    {
        private StallEntities _db = new StallEntities();

        // GET api/<controller>
        [HttpGet]
        public OperationResult<string> Suburb2Area([FromUri]string country, [FromUri]string city, [FromUri]string suburb)
        {
            var result = new OperationResult<string>(true);
            if(country.ToUpper().Equals("NZ") && city.ToUpper().Equals("AUCKLAND"))
            {
                //get surburb for auckland
                var suburbRec = Suburb.Find(suburb, city, country, _db);
                if (suburbRec != null)
                {
                    result.Data = suburbRec.Area;
                }
                else
                {
                    result.Succeeded = false;
                    result.Message = "不能识别Suburb";
                }
            }
            else
            {
                result.Data = city;
            }

          
            return result;
        }
    }
}