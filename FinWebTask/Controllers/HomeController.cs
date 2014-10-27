using FinWebTask.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace FinWebTask.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://195.128.78.52/GAZP_141027_141027.csv?market=1&em=16842&code=GAZP&df=27&mf=9&yf=2014&from=27.10.2014&dt=27&mt=9&yt=2014&to=27.10.2014&p=7&f=GAZP_141027_141027&e=.csv&cn=GAZP&dtf=1&tmf=1&MSOR=1&mstime=on&mstimever=1&sep=3&sep2=1&datf=1&at=1");
            var response = request.GetResponse();
            var result = "";
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var csv = new CsvHelper.CsvReader(streamReader);
                csv.Configuration.Delimiter = ";";
                while (csv.Read()) {
                    var asd = csv.GetField<string>("<TICKER>");
                }
            }
            return View();
        }
    }
}
