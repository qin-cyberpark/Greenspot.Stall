using System.Linq;
using Greenspot.Configuration;
using Greenspot.Stall.Utilities;

namespace Greenspot.Stall.Models
{
    public partial class SuburbDistance
    {
        private static string BuilId(string depCountry, string depCity, string depSuburb,
                                        string destCountry, string destCity, string destSuburb)
        {
            if (string.IsNullOrEmpty(depCountry) || string.IsNullOrEmpty(destCountry)
                || string.IsNullOrEmpty(depCity) || string.IsNullOrEmpty(depSuburb)
                || string.IsNullOrEmpty(destCity) || string.IsNullOrEmpty(destSuburb))
            {
                return null;
            }

            var dep = string.Format("{0}:{1}:{2}", depCountry, depCity, depSuburb.Replace(' ', '-'));
            var dest = string.Format("{0}:{1}:{2}", destCountry, destCity, destSuburb.Replace(' ', '-'));
            var bigger = string.Compare(dep, dest, true);

            //return string.Format("{0}-{1}", bigger > 0 ? dep : dest, bigger > 0 ? dest : dep).ToLower();
            return string.Format("{0}&{1}", dep, dest).ToLower();
        }


        public static int? GetDistance(string depCountry, string depCity, string depSuburb,
                                                string destCountry, string destCity, string destSuburb)
        {
            //get from db
            using (var db = new StallEntities())
            {
                var id = BuilId(depCountry, depCity, depSuburb, destCountry, destCity, destSuburb);
                var dbResult = GetDistance(id, db);
                if (dbResult != null)
                {
                    return dbResult;
                }

                //call google map api
                var glResult = GoogleMapHelper.GetSuburbDistanceFromGoogleMapApi(depCountry, depCity, depSuburb, destCountry, destCity, destSuburb,
                    GreenspotConfiguration.AccessAccounts["google.map"].Secret);
                if (glResult == null)
                {
                    StallApplication.SysError($"[GOOGLE DISTANCE]failed to get distance {depCountry},{depCity},{depSuburb} to {destCountry},{destCity},{destSuburb}");
                    return null;
                }
                else
                {
                    //save to db
                    var distance = new SuburbDistance()
                    {
                        ID = id,
                        DepartureCountryCode = depCountry,
                        DepartureCity = depCity,
                        DepartureSuburb = depSuburb,

                        DestinationCountryCode = destCountry,
                        DestinationCity = destCity,
                        DestinationSuburb = destSuburb,

                        Meters = glResult.Value
                    };

                    db.SuburbDistances.Add(distance);
                    db.SaveChanges();

                    return glResult.Value;
                }
            }
        }

        public static int? GetDistance(string id, StallEntities db)
        {
            var rec = FindById(id, db);
            if (rec != null && rec.Meters != null)
            {
                return rec.Meters;
            }
            else
            {
                return null;
            }
        }

        public static SuburbDistance FindById(string id, StallEntities db)
        {
            return db.SuburbDistances.FirstOrDefault(x => x.ID.Equals(id));
        }
    }
}