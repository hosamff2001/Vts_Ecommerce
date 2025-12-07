namespace Vts_Ecommerce.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class adddddd : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "CostPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Products", "SellingPrice", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            AddColumn("dbo.Customers", "Notes", c => c.String(maxLength: 500));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Customers", "Notes");
            DropColumn("dbo.Products", "SellingPrice");
            DropColumn("dbo.Products", "CostPrice");
        }
    }
}
