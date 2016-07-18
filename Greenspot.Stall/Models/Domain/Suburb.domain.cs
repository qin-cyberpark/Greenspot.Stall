using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Greenspot.Stall.Models
{
    public partial class Suburb
    {
        public static Suburb Find(string name, string city, string countryCode, StallEntities db)
        {
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(city) || string.IsNullOrEmpty(countryCode))
            {
                return null;
            }
            return db.Suburbs.FirstOrDefault(x => x.Name.Equals(name) && x.City.Equals(city) && x.CountryCode.Equals(countryCode));
        }
    }
}