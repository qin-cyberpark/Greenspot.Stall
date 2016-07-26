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
            name = name.ToLower();
            city = city.ToLower();
            countryCode = countryCode.ToLower();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(city) || string.IsNullOrEmpty(countryCode))
            {
                return null;
            }
            return db.Suburbs.FirstOrDefault(x => x.Name.ToLower().Equals(name) && x.City.ToLower().Equals(city) 
                                                && x.CountryCode.ToLower().Equals(countryCode));
        }
    }
}