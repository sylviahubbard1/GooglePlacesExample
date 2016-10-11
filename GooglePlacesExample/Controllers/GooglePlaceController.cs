
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using GooglePlacesExample.Controllers;
using GooglePlacesExample.Models;

namespace GooglePlacesExample.Controllers
{
    public class GooglePlaceController : Controller
    {
        private readonly string APIKEY = "";

        
        public ActionResult Index()
        {
            return View();
        }
        [HttpGet]
        public ActionResult Search()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Search(decimal latitude = 0, decimal longitude = 0, int radius = 0, string type = "", string name = "")
        {

            //string searchUrl = @"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=" + latitude + "," + longitude +
            // @"&radius=" + radius + $@"&type={type}" + @"&name=" + name + @"&key=" +APIKEY;

            //searchUrl = @"https://maps.googleapis.com/maps/api/place/nearbysearch/json?location=-33.8670522,151.1957362&radius=500&type=restaurant&name=cruise&key=AIzaSyDilaKqynhjELXyHU0vZJU3L8nNUw2T9S0";
          String  searchUrl = @"http://forecast.weather.gov/MapClick.php?lat=" + latitude + "&lon=" + longitude + "&FcstType=json";
            JObject jobject = MakeRequest(searchUrl);


              var results = jobject["data"]["text"].ToList();

            List<string> lst = new List<string>();

            foreach (var r in results)
            {
                lst.Add(r.ToString());
            }
            ViewBag.results = lst;

            ViewBag.location = jobject["location"]["areaDescription"].ToString();


            //List<PassThroughPart> parts = new List<PassThroughPart>();

            // ViewBag.result = jobject.Root.ToString();

            //foreach (var r in results)
            //{

            //    PassThroughPart p = new PassThroughPart();
            //    p.lat = (decimal)r["geometry"]["location"]["lat"];
            //    p.lng = (decimal)r["geometry"]["location"]["lng"];


            //    p.name = (string)r["name"];
            //    p.icon = (string)r["icon"];
            //    if (r["rating"] != null)
            //    {
            //        p.rating = (decimal)r["rating"];
            //    }
            //    p.types = r["types"].ToString().Split(',').ToList();

            //    parts.Add(p);
            //}
            //ViewBag.Latitude = latitude;
            //ViewBag.Longitude = longitude;
            //ViewBag.Results = parts;

            return View("SearchResults");
        }

        public JObject MakeRequest(string requestUrl)
        {
            JObject o;
            try
            {

                HttpWebRequest request = WebRequest.Create(requestUrl) as HttpWebRequest;
                request.UserAgent = @"User-Agent: Mozilla/5.0 (Windows NT 10.0; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/53.0.2785.116 Safari/537.36";
                request.UseDefaultCredentials = true;
                request.Proxy.Credentials = System.Net.CredentialCache.DefaultCredentials;
                
                using (HttpWebResponse response = request.GetResponse() as HttpWebResponse)
                {
                    if (response.StatusCode != HttpStatusCode.OK)
                    {
                        //o = JObject.Parse("{error:\"Server error\"}");
                        throw new Exception(String.Format("Server error (HTTP {0}: {1}).", response.StatusCode, response.StatusDescription));
                    }
                    using (var reader = new StreamReader(response.GetResponseStream()))
                    {
                        string result = reader.ReadToEnd();

                        o = JObject.Parse(result);
                        return o;
                    }
                }
                //http://forecast.weather.gov/MapClick.php?lat=38.4247341&lon=-86.9624086&FcstType=json
            }
            catch (Exception e)
            {
                // catch exception and log it
                o = JObject.Parse("{error:\"" + e.Message + "\"}");

                return o;
            }

        }


    }

}