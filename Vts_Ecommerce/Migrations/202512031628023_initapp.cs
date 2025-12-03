namespace Vts_Ecommerce.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class initapp : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Categories",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Description = c.String(maxLength: 500),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Description = c.String(maxLength: 1000),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        StockQuantity = c.Int(nullable: false),
                        CategoryId = c.Int(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Categories", t => t.CategoryId)
                .Index(t => t.CategoryId);
            
            CreateTable(
                "dbo.SalesInvoiceItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InvoiceId = c.Int(nullable: false),
                        ProductId = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        UnitPrice = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ItemDiscount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        LineTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.SalesInvoices", t => t.InvoiceId, cascadeDelete: true)
                .ForeignKey("dbo.Products", t => t.ProductId)
                .Index(t => t.InvoiceId)
                .Index(t => t.ProductId);
            
            CreateTable(
                "dbo.SalesInvoices",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        InvoiceNumber = c.String(nullable: false, maxLength: 50),
                        CustomerId = c.Int(nullable: false),
                        InvoiceDate = c.DateTime(nullable: false),
                        SubTotal = c.Decimal(nullable: false, precision: 18, scale: 2),
                        ItemDiscount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        InvoiceDiscount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Total = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreatedBy = c.Int(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.CreatedBy)
                .ForeignKey("dbo.Customers", t => t.CustomerId)
                .Index(t => t.InvoiceNumber, unique: true)
                .Index(t => t.CustomerId)
                .Index(t => t.CreatedBy);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(nullable: false, maxLength: 50),
                        Password = c.String(nullable: false, maxLength: 500),
                        Email = c.String(maxLength: 100),
                        IsActive = c.Boolean(nullable: false),
                        CreatedDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.UserSessions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserId = c.Int(nullable: false),
                        SessionId = c.String(nullable: false, maxLength: 100),
                        DeviceInfo = c.String(maxLength: 500),
                        LoginTime = c.DateTime(nullable: false),
                        LastActivityTime = c.DateTime(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.SessionId, unique: true);
            
            CreateTable(
                "dbo.Customers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 100),
                        Email = c.String(maxLength: 100),
                        Phone = c.String(maxLength: 20),
                        Address = c.String(maxLength: 500),
                        IsActive = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Products", "CategoryId", "dbo.Categories");
            DropForeignKey("dbo.SalesInvoiceItems", "ProductId", "dbo.Products");
            DropForeignKey("dbo.SalesInvoiceItems", "InvoiceId", "dbo.SalesInvoices");
            DropForeignKey("dbo.SalesInvoices", "CustomerId", "dbo.Customers");
            DropForeignKey("dbo.UserSessions", "UserId", "dbo.Users");
            DropForeignKey("dbo.SalesInvoices", "CreatedBy", "dbo.Users");
            DropIndex("dbo.UserSessions", new[] { "SessionId" });
            DropIndex("dbo.UserSessions", new[] { "UserId" });
            DropIndex("dbo.SalesInvoices", new[] { "CreatedBy" });
            DropIndex("dbo.SalesInvoices", new[] { "CustomerId" });
            DropIndex("dbo.SalesInvoices", new[] { "InvoiceNumber" });
            DropIndex("dbo.SalesInvoiceItems", new[] { "ProductId" });
            DropIndex("dbo.SalesInvoiceItems", new[] { "InvoiceId" });
            DropIndex("dbo.Products", new[] { "CategoryId" });
            DropTable("dbo.Customers");
            DropTable("dbo.UserSessions");
            DropTable("dbo.Users");
            DropTable("dbo.SalesInvoices");
            DropTable("dbo.SalesInvoiceItems");
            DropTable("dbo.Products");
            DropTable("dbo.Categories");
        }
    }
}
