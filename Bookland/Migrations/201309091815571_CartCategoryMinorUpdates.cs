namespace Bookland.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class CartCategoryMinorUpdates : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Category", "CategoryDescription", c => c.String(maxLength: 250));
            DropColumn("dbo.Cart", "UserID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Cart", "UserID", c => c.Int(nullable: false));
            AlterColumn("dbo.Category", "CategoryDescription", c => c.String());
        }
    }
}
