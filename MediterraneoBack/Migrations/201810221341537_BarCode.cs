namespace MediterraneoBack.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BarCode : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.OrderDetails", "BarCode", c => c.String(nullable: false, maxLength: 13));
        }
        
        public override void Down()
        {
            DropColumn("dbo.OrderDetails", "BarCode");
        }
    }
}
