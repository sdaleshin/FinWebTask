using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinWebTask.DTOs
{
    public class TickerModel
    {
        public string Ticker;
        public DateTime Date { get; set; }
        public double AVG { get; set; }
        public double Norm { get; set; }
        public int Index { get; set; }
    }
}