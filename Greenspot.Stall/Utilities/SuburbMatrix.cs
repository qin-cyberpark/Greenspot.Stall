using Greenspot.Stall.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data.Entity.Migrations;
using System.Linq;
using System.Net;
using System.Web;
using System.Data.Entity;
using System.Data;
using System.Threading.Tasks;

namespace Greenspot.Stall.Utilities
{
    public static class DistanceMatrix
    {
        private static string BuildKey(string region, string ori, string dest, string countryCode = "NZ")
        {
            if (string.IsNullOrEmpty(countryCode) || string.IsNullOrEmpty(region)
                || string.IsNullOrEmpty(ori) || string.IsNullOrEmpty(dest))
            {
                return null;
            }

            var bigger = string.Compare(ori, dest, true);

            return string.Format("{0}::{1}::{2}|{3}", countryCode, region, bigger > 0 ? ori : dest, bigger > 0 ? dest : ori).ToUpper();
        }

        public static void InitDB()
        {
            while (true)
            {
                using (var db = new StallEntities())
                {
                    var pairs = db.SuburbDistances.Where(x => x.Meters == null && !x.OriginSuburb.Equals(x.DestinationSuburb)).Take(100).ToList();
                    if (pairs == null || pairs.Count == 0)
                    {
                        return;
                    }

                    foreach (var p in pairs)
                    {
                        p.Meters = GetSuburbDistanceFromGoogleMapApi(p.City, p.OriginSuburb, p.DestinationSuburb);
                        if (p.Meters != null)
                        {
                            //store distance
                            Console.WriteLine("{0},{1}", p.OriginSuburb, p.DestinationSuburb);
                        }else
                        {
                            Task.Delay(5000);
                            break;
                        }
                    }
                    db.SaveChanges();
                }
            }
        }

        public static int? GetSuburbDistaince(bool isGoogleApi, string city, string ori, string dest, string countryCode = "NZ")
        {
            if (!isGoogleApi)
            {

                return GetSuburbDistanceFromDB(city, ori, dest, countryCode);
            }
            else
            {
                //get from google map
                //check whether suburbs are exist
                using (var db = new StallEntities())
                {
                    //check ori
                    var oriSub = db.Suburbs.FirstOrDefault(x => x.CountryCode.ToUpper().Equals(countryCode.ToUpper())
                                                    && x.City.ToUpper().Equals(city.ToUpper())
                                                    && x.Name.ToUpper().Equals(ori.ToUpper()));
                    if (oriSub == null)
                    {
                        return null;
                    }

                    //check dest
                    var destSub = db.Suburbs.FirstOrDefault(x => x.CountryCode.ToUpper().Equals(countryCode.ToUpper())
                                                       && x.City.ToUpper().Equals(city.ToUpper())
                                                       && x.Name.ToUpper().Equals(dest.ToUpper()));
                    if (destSub == null)
                    {
                        return null;
                    }

                    //call google map api
                    var distance = GetSuburbDistanceFromGoogleMapApi(city, ori, dest, countryCode);
                    if (distance == null)
                    {
                        return null;
                    }

                    return distance.Value;
                }
            }

        }

        public static int? GetSuburbDistanceFromDB(string city, string ori, string dest, string countryCode = "NZ")
        {
            using (var db = new StallEntities())
            {
                var rec = db.SuburbDistances.FirstOrDefault(x => x.CountryCode.ToUpper().Equals(countryCode.ToUpper())
                                                    && x.City.ToUpper().Equals(city.ToUpper())
                                                    && x.OriginSuburb.ToUpper().Equals(ori.ToUpper())
                                                    && x.DestinationSuburb.ToUpper().Equals(dest.ToUpper()));
                if (rec != null && rec.Meters != null)
                {
                    return rec.Meters;
                }
                else
                {
                    return null;
                }
            }
        }

        public static int? GetSuburbDistanceFromGoogleMapApi(string region, string ori, string dest, string countryCode = "NZ")
        {
            string reqUrlPattern = "https://maps.googleapis.com/maps/api/directions/json?origin={0}&destination={1}&key={2}";
            var reqUrl = string.Format(reqUrlPattern, HttpUtility.UrlEncode(string.Format("{0},{1},{2}", ori, region, countryCode)),
                HttpUtility.UrlEncode(string.Format("{0},{1},{2}", dest, region, countryCode)), "");
            using (var client = new WebClient())
            {
                var resp = JsonConvert.DeserializeObject<DirectionResponse>(client.DownloadString(reqUrl));
                if (resp.Status.Equals("OK") && resp.Routes.Count > 0)
                {
                    return resp.Routes[0].Legs[0].Distance.Value;
                }
                else
                {
                    return null;
                }
            }
        }

        #region internal class

        private class GeocodedWaypoint
        {

            [JsonProperty("geocoder_status")]
            public string GeocoderStatus { get; set; }

            [JsonProperty("place_id")]
            public string PlaceId { get; set; }

            [JsonProperty("types")]
            public IList<string> Types { get; set; }
        }

        private class Bounds
        {

            [JsonProperty("northeast")]
            public Location Northeast { get; set; }

            [JsonProperty("southwest")]
            public Location Southwest { get; set; }
        }

        private class Distance
        {

            [JsonProperty("text")]
            public string Text { get; set; }

            [JsonProperty("value")]
            public int Value { get; set; }
        }

        private class Duration
        {

            [JsonProperty("text")]
            public string Text { get; set; }

            [JsonProperty("value")]
            public int Value { get; set; }
        }

        private class Location
        {

            [JsonProperty("lat")]
            public double Lat { get; set; }

            [JsonProperty("lng")]
            public double Lng { get; set; }
        }

        private class Polyline
        {

            [JsonProperty("points")]
            public string Points { get; set; }
        }

        private class Step
        {

            [JsonProperty("distance")]
            public Distance Distance { get; set; }

            [JsonProperty("duration")]
            public Duration Duration { get; set; }

            [JsonProperty("end_location")]
            public Location EndLocation { get; set; }

            [JsonProperty("html_instructions")]
            public string HtmlInstructions { get; set; }

            [JsonProperty("polyline")]
            public Polyline Polyline { get; set; }

            [JsonProperty("start_location")]
            public Location StartLocation { get; set; }

            [JsonProperty("travel_mode")]
            public string TravelMode { get; set; }

            [JsonProperty("maneuver")]
            public string Maneuver { get; set; }
        }

        private class Leg
        {

            [JsonProperty("distance")]
            public Distance Distance { get; set; }

            [JsonProperty("duration")]
            public Duration Duration { get; set; }

            [JsonProperty("end_address")]
            public string EndAddress { get; set; }

            [JsonProperty("end_location")]
            public Location EndLocation { get; set; }

            [JsonProperty("start_address")]
            public string StartAddress { get; set; }

            [JsonProperty("start_location")]
            public Location StartLocation { get; set; }

            [JsonProperty("steps")]
            public IList<Step> Steps { get; set; }

            [JsonProperty("traffic_speed_entry")]
            public IList<object> TrafficSpeedEntry { get; set; }

            [JsonProperty("via_waypoint")]
            public IList<object> ViaWaypoint { get; set; }
        }

        private class OverviewPolyline
        {

            [JsonProperty("points")]
            public string Points { get; set; }
        }

        private class Route
        {

            [JsonProperty("bounds")]
            public Bounds Bounds { get; set; }

            [JsonProperty("copyrights")]
            public string Copyrights { get; set; }

            [JsonProperty("legs")]
            public IList<Leg> Legs { get; set; }

            [JsonProperty("overview_polyline")]
            public OverviewPolyline OverviewPolyline { get; set; }

            [JsonProperty("summary")]
            public string Summary { get; set; }

            [JsonProperty("warnings")]
            public IList<object> Warnings { get; set; }

            [JsonProperty("waypoint_order")]
            public IList<object> WaypointOrder { get; set; }
        }

        private class DirectionResponse
        {
            [JsonProperty("geocoded_waypoints")]
            public IList<GeocodedWaypoint> GeocodedWaypoints { get; set; }

            [JsonProperty("routes")]
            public IList<Route> Routes { get; set; }

            [JsonProperty("status")]
            public string Status { get; set; }
        }

        #endregion
    }
}