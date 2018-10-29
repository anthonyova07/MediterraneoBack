namespace MediterraneoBack.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RNC : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Salespersons", "RNC", c => c.String(nullable: false, maxLength: 11));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Salespersons", "RNC");
        }
    }
}
