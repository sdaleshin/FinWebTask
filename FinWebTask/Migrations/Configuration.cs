namespace FinWebTask.Migrations
{
    using FinWebTask.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<FinWebTask.Models.FinContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(FinWebTask.Models.FinContext context)
        {
            context.TickerInfo.AddOrUpdate(
                p => p.Ticker,
                new TickerInfo()
                {
                    Ticker = "TEST",
                    Vol = 123,
                    Per = 123,
                    Open = 123,
                    Low = 123,
                    High = 123,
                    Close = 123,
                    DateTime = DateTime.Now
                });
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data. E.g.
            //
            //    context.People.AddOrUpdate(
            //      p => p.FullName,
            //      new Person { FullName = "Andrew Peters" },
            //      new Person { FullName = "Brice Lambson" },
            //      new Person { FullName = "Rowan Miller" }
            //    );
            //
        }
    }
}
