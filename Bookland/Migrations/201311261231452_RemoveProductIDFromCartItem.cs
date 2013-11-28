namespace Bookland.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RemoveProductIDFromCartItem : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.CartItem", "ProductID", "dbo.Product");
            DropIndex("dbo.CartItem", new[] { "ProductID" });
            RenameColumn(table: "dbo.CartItem", name: "ProductID", newName: "Product_ProductID");
            AddForeignKey("dbo.CartItem", "Product_ProductID", "dbo.Product", "ProductID");
            CreateIndex("dbo.CartItem", "Product_ProductID");
        }
        
        public override void Down()
        {
            DropIndex("dbo.CartItem", new[] { "Product_ProductID" });
            DropForeignKey("dbo.CartItem", "Product_ProductID", "dbo.Product");
            RenameColumn(table: "dbo.CartItem", name: "Product_ProductID", newName: "ProductID");
            CreateIndex("dbo.CartItem", "ProductID");
            AddForeignKey("dbo.CartItem", "ProductID", "dbo.Product", "ProductID", cascadeDelete: true);
        }
    }
}
