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
        private static string BuildKey(string depCountry, string depCity, string depSuburb,
                                        string destCountry, string destCity, string destSuburb)
        {
            if (string.IsNullOrEmpty(depCountry) || string.IsNullOrEmpty(destCountry)
                || string.IsNullOrEmpty(depCity) || string.IsNullOrEmpty(depSuburb)
                || string.IsNullOrEmpty(destCity) || string.IsNullOrEmpty(destSuburb))
            {
                return null;
            }

            var dep = string.Format("{0}::{1}::{2}", depCountry, depCity, depSuburb);
            var dest = string.Format("{0}::{1}::{2}", destCountry, destCity, destSuburb);
            var bigger = string.Compare(dep, dest, true);

            //return string.Format("{0}-{1}", bigger > 0 ? dep : dest, bigger > 0 ? dest : dep).ToLower();
            return string.Format("{0}-{1}", dep, dest).ToLower();
        }


        public static int? GetSuburbDistaince(string depCountry, string depCity, string depSuburb,
                                                string destCountry, string destCity, string destSuburb)
        {
            //get from db
            var dbResult = GetSuburbDistanceFromDB(depCountry, depCity, depSuburb, destCountry, destCity, destSuburb);
            if (dbResult != null)
            {
                return dbResult;
            }

            //call google map api
            var glResult = GetSuburbDistanceFromGoogleMapApi(depCountry, depCity, depSuburb, destCountry, destCity, destSuburb);
            if (glResult == null)
            {
                return null;
            }

            //save to db
            using (var db = new StallEntities())
            {
                var distance = new SuburbDistance()
                {
                    ID = BuildKey(depCountry, depCity, depSuburb, destCountry, destCity, destSuburb),
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

        public static int? GetSuburbDistanceFromDB(string depCountry, string depCity, string depSuburb,
                                                    string destCountry, string destCity, string destSuburb)
        {
            using (var db = new StallEntities())
            {
                var key = BuildKey(depCountry, depCity, depSuburb, destCountry, destCity, destSuburb);
                var rec = db.SuburbDistances.FirstOrDefault(x => x.ID.Equals(key));
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

        public static int? GetSuburbDistanceFromGoogleMapApi(string depCountry, string depCity, string depSuburb,
                                                    string destCountry, string destCity, string destSuburb, string key = null)
        {
            depCountry = depCountry.ToLower();
            depCity = depCity.ToLower();
            depSuburb = depSuburb.ToLower();

            destCountry = destCountry.ToLower();
            destCity = destCity.ToLower();
            destSuburb = destSuburb.ToLower();

            string reqUrlPattern = "https://maps.googleapis.com/maps/api/directions/json?origin={0}&destination={1}&key={2}";
            var reqUrl = string.Format(reqUrlPattern, HttpUtility.UrlEncode(string.Format("{0},{1},{2}", depSuburb, depCity, depCountry)),
                HttpUtility.UrlEncode(string.Format("{0},{1},{2}", destSuburb, destCity, destCountry)), key);
            using (var client = new WebClient())
            {
                var resp = JsonConvert.DeserializeObject<DirectionResponse>(client.DownloadString(reqUrl));
                if (resp.Status.Equals("OK") && resp.Routes.Count > 0)
                {
                    //check start address
                    var startAddress = resp.Routes[0].Legs[0].StartAddress.ToLower();
                    if (!startAddress.Contains(depCity) || !startAddress.Contains(depSuburb))
                    {
                        return null;
                    }

                    //check start address
                    var endAddress = resp.Routes[0].Legs[0].EndAddress.ToLower();
                    if (!endAddress.Contains(destCity) || !endAddress.Contains(destSuburb))
                    {
                        return null;
                    }

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