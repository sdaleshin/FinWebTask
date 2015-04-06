using FinWebTask.DTOs;
using FinWebTask.Managers;
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
        private TickerManager tickerManager;
        public HomeController() : base() {
            context = new FinContext();
            tickerManager = new TickerManager(context);
        }
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";
            ViewBag.TickerData = tickerManager.GetTickers().OrderByDescending(s => s.Date).Take(50); 

            return View();
        }
        [HttpPost]
        public ActionResult GetPredictionData(ConditionModel model) {
            var tickers = tickerManager.GetTickers().Where(s => s.Date >= model.StartDate && s.Date <= model.EndDate && s.Ticker == model.Ticker);
            return Json(new { tickers = tickers });
        }
        protected override void Dispose(bool disposing)
        {
            context.Dispose();
            base.Dispose(disposing);
        }
    }
}
