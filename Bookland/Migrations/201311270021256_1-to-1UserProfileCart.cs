namespace Bookland.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class _1to1UserProfileCart : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.UserProfile", "Cart_CartID", "dbo.Cart");
            DropIndex("dbo.UserProfile", new[] { "Cart_CartID" });
            AddColumn("dbo.Cart", "UserID", c => c.Int(nullable: false));
            AddForeignKey("dbo.Cart", "UserID", "dbo.UserProfile", "UserID");
            CreateIndex("dbo.Cart", "UserID");
            DropColumn("dbo.UserProfile", "Cart_CartID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.UserProfile", "Cart_CartID", c => c.Int());
            DropIndex("dbo.Cart", new[] { "UserID" });
            DropForeignKey("dbo.Cart", "UserID", "dbo.UserProfile");
            DropColumn("dbo.Cart", "UserID");
            CreateIndex("dbo.UserProfile", "Cart_CartID");
            AddForeignKey("dbo.UserProfile", "Cart_CartID", "dbo.Cart", "CartID");
        }
    }
}
