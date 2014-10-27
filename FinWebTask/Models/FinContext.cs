using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;

namespace FinWebTask.Models
{
    public class FinContext : DbContext
    {
        public FinContext()
            : this("name=FinContext")
        {

        }

        public FinContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            Configure();
        }

        private void  Configure(){
        
        }

        public IDbSet<TickerInfo> TickerInfo { get; set; }
    }
}