using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace FinWebTask.DTOs
{
    public class ConditionModel
    {
        public string Ticker { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}