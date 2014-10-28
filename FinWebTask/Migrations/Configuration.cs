namespace FinWebTask.Migrations
{
    using FinWebTask.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;

    internal sealed class Configuration : DbMigrationsConfiguration<FinWebTask.Models.FinContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(FinWebTask.Models.FinContext context)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://195.128.78.52/GAZP_121027_141027.csv?market=1&em=16842&code=GAZP&df=27&mf=9&yf=2012&from=01.01.2012&dt=27&mt=9&yt=" +DateTime.Now.Year +  "&to=" + DateTime.Now.ToString("dd.MM.yyyy") + "&p=7&f=GAZP_141027_141027&e=.csv&cn=GAZP&dtf=1&tmf=1&MSOR=1&mstime=on&mstimever=1&sep=3&sep2=1&datf=1&at=1");
            var response = request.GetResponse();
            using (var streamReader = new StreamReader(response.GetResponseStream()))
            {
                var csv = new CsvHelper.CsvReader(streamReader);
                csv.Configuration.Delimiter = ";";
                NumberFormatInfo provider = new NumberFormatInfo();
                provider.NumberGroupSeparator = ".";
                while (csv.Read())
                {
                    var tickerInfo = new TickerInfo()
                    {
                        Ticker = csv.GetField<string>("<TICKER>"),
                        Per = csv.GetField<decimal>("<PER>"),
                        Open = Convert.ToDouble(csv.GetField<string>("<OPEN>"), provider),
                        High = Convert.ToDouble(csv.GetField<string>("<HIGH>"), provider),
                        Low = Convert.ToDouble(csv.GetField<string>("<LOW>"), provider),
                        Close = Convert.ToDouble(csv.GetField<string>("<CLOSE>"), provider),
                        Vol = csv.GetField<decimal>("<VOL>"),
                        DateTime = DateTime.ParseExact(csv.GetField<string>("<DATE>") + csv.GetField<string>("<TIME>"), "yyyyMMddHHmmss", provider)
                    };

                    context.TickerInfo.AddOrUpdate(
                        p => new { p.Ticker, p.DateTime },
                        tickerInfo  
                    );

                }
                context.SaveChanges();
            }
        }
    }
}
