using FinWebTask.DTOs;
using FinWebTask.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinWebTask.Managers
{
    public class TickerManager
    {
        private readonly FinContext context;

        public TickerManager(FinContext finContext)
        {
            context = finContext;
        }

        public List<TickerModel> GetTickers(){
            return context.TickerInfo.GroupBy(s => new { s.DateTime.Year, s.DateTime.Month, s.DateTime.Day }).Select(GetTickerModel).ToList();
        }

        private TickerModel GetTickerModel(IGrouping<object,TickerInfo> group)
        {
            var retVal = new TickerModel();
            retVal.Date = group.FirstOrDefault().DateTime.Date;
            retVal.AVG = group.Average(s => s.AVG);
            retVal.Ticker = group.FirstOrDefault().Ticker;
            return retVal;
        }
    }
}