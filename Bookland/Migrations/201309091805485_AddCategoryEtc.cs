namespace Bookland.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddCategoryEtc : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Category",
                c => new
                    {
                        CategoryID = c.Int(nullable: false, identity: true),
                        CategoryName = c.String(maxLength: 50),
                        CategoryDescription = c.String(),
                    })
                .PrimaryKey(t => t.CategoryID);
            
            CreateTable(
                "dbo.CategoryRelationship",
                c => new
                    {
                        ParentID = c.Int(nullable: false),
                        ChildID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.ParentID, t.ChildID })
                .ForeignKey("dbo.Category", t => t.ParentID)
                .ForeignKey("dbo.Category", t => t.ChildID)
                .Index(t => t.ParentID)
                .Index(t => t.ChildID);
            
            AddColumn("dbo.Product", "Category_CategoryID", c => c.Int());
            AlterColumn("dbo.Product", "Price", c => c.Decimal(nullable: false, storeType: "money"));
            AddForeignKey("dbo.Product", "Category_CategoryID", "dbo.Category", "CategoryID");
            CreateIndex("dbo.Product", "Category_CategoryID");
            DropColumn("dbo.Cart", "CartItemID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Cart", "CartItemID", c => c.Int(nullable: false));
            DropIndex("dbo.CategoryRelationship", new[] { "ChildID" });
            DropIndex("dbo.CategoryRelationship", new[] { "ParentID" });
            DropIndex("dbo.Product", new[] { "Category_CategoryID" });
            DropForeignKey("dbo.CategoryRelationship", "ChildID", "dbo.Category");
            DropForeignKey("dbo.CategoryRelationship", "ParentID", "dbo.Category");
            DropForeignKey("dbo.Product", "Category_CategoryID", "dbo.Category");
            AlterColumn("dbo.Product", "Price", c => c.Decimal(nullable: false, precision: 18, scale: 2));
            DropColumn("dbo.Product", "Category_CategoryID");
            DropTable("dbo.CategoryRelationship");
            DropTable("dbo.Category");
        }
    }
}
