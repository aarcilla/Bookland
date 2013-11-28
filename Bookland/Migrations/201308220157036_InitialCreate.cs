namespace Bookland.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Product",
                c => new
                    {
                        ProductID = c.Int(nullable: false, identity: true),
                        Name = c.String(maxLength: 150),
                        Description = c.String(maxLength: 250),
                        Year = c.Int(nullable: false),
                        Price = c.Decimal(nullable: false, precision: 18, scale: 2),
                        DateAdded = c.DateTime(nullable: false),
                        ImageData = c.Binary(),
                        ImageMimeType = c.String(),
                    })
                .PrimaryKey(t => t.ProductID);
            
            CreateTable(
                "dbo.CartItem",
                c => new
                    {
                        CartItemID = c.Int(nullable: false, identity: true),
                        ProductID = c.Int(nullable: false),
                        Quantity = c.Int(nullable: false),
                        Cart_CartID = c.Int(),
                    })
                .PrimaryKey(t => t.CartItemID)
                .ForeignKey("dbo.Product", t => t.ProductID, cascadeDelete: true)
                .ForeignKey("dbo.Cart", t => t.Cart_CartID)
                .Index(t => t.ProductID)
                .Index(t => t.Cart_CartID);
            
            CreateTable(
                "dbo.Cart",
                c => new
                    {
                        CartID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        CartItemID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.CartID);
            
            CreateTable(
                "dbo.UserProfile",
                c => new
                    {
                        UserID = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Email = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                        Cart_CartID = c.Int(),
                    })
                .PrimaryKey(t => t.UserID)
                .ForeignKey("dbo.Cart", t => t.Cart_CartID)
                .Index(t => t.Cart_CartID);
            
            CreateTable(
                "dbo.Address",
                c => new
                    {
                        AddressID = c.Int(nullable: false, identity: true),
                        StreetLine1 = c.String(),
                        StreetLine2 = c.String(),
                        City = c.String(),
                        State = c.String(),
                        Country = c.String(),
                        Postcode = c.Int(nullable: false),
                        UserID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.AddressID)
                .ForeignKey("dbo.UserProfile", t => t.UserID)
                .Index(t => t.UserID);
            
        }
        
        public override void Down()
        {
            DropIndex("dbo.Address", new[] { "UserID" });
            DropIndex("dbo.UserProfile", new[] { "Cart_CartID" });
            DropIndex("dbo.CartItem", new[] { "Cart_CartID" });
            DropIndex("dbo.CartItem", new[] { "ProductID" });
            DropForeignKey("dbo.Address", "UserID", "dbo.UserProfile");
            DropForeignKey("dbo.UserProfile", "Cart_CartID", "dbo.Cart");
            DropForeignKey("dbo.CartItem", "Cart_CartID", "dbo.Cart");
            DropForeignKey("dbo.CartItem", "ProductID", "dbo.Product");
            DropTable("dbo.Address");
            DropTable("dbo.UserProfile");
            DropTable("dbo.Cart");
            DropTable("dbo.CartItem");
            DropTable("dbo.Product");
        }
    }
}
