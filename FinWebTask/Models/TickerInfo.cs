using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace FinWebTask.Models
{
    [Table("TickerInfo")]
    public class TickerInfo
    {
        public TickerInfo()
        {
            Id = Guid.NewGuid();
        }
        [Key]
        public Guid Id { get; set; }
        public string Ticker { get; set; }
        public decimal Per { get; set; }
        public DateTime DateTime { get; set; }
        public double Open { get; set; }
        public double High { get; set; }
        public double Low { get; set; }
        public double Close { get; set; }
        public decimal Vol { get; set; }
        public double AVG
        {
            get
            {
                return (Open + High + Low + Close) / 4.0;
            }
        }
    }
}