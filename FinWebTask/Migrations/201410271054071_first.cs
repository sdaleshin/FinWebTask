namespace FinWebTask.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class first : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.TickerInfoes",
                c => new
                    {
                        Id = c.Guid(nullable: false),
                        Ticker = c.String(),
                        Per = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateTime = c.DateTime(nullable: false),
                        Open = c.Double(nullable: false),
                        High = c.Double(nullable: false),
                        Low = c.Double(nullable: false),
                        Close = c.Double(nullable: false),
                        Vol = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.TickerInfoes");
        }
    }
}
