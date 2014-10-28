using FinWebTask.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace FinWebTask.Controllers
{
    public class HomeController : Controller
    {
        private FinContext context;
        public HomeController() : base() {
            context = new FinContext();
        }
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            ViewBag.TickerData = context.TickerInfo.OrderByDescending(s => s.DateTime).Take(50).ToList();

            return View();
        }
        protected override void Dispose(bool disposing)
        {
            context.Dispose();
            base.Dispose(disposing);
        }
    }
}
