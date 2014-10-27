using FinWebTask.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace FinWebTask.Controllers
{
    public class TickerInfoController : ApiController
    {
        private FinContext context;
        public TickerInfoController()
            : base()
        {
            context = new FinContext();
        }
        // GET api/values
        public IEnumerable<TickerInfo> Get()
        {
            return context.TickerInfo;
        }

        // GET api/values/5
        public TickerInfo Get(Guid id)
        {
            return context.TickerInfo.FirstOrDefault(s => s.Id == id);
        }

        // POST api/values
        public void Post([FromBody]string value)
        {
        }

        // PUT api/values/5
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/values/5
        public void Delete(int id)
        {
        }

        protected override void Dispose(bool disposing)
        {
            context.Dispose();
            base.Dispose(disposing);
        }
    }
}
