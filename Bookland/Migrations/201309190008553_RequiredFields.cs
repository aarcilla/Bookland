namespace Bookland.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class RequiredFields : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Product", "Name", c => c.String(nullable: false, maxLength: 150));
            AlterColumn("dbo.Category", "CategoryName", c => c.String(nullable: false, maxLength: 50));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Category", "CategoryName", c => c.String(maxLength: 50));
            AlterColumn("dbo.Product", "Name", c => c.String(maxLength: 150));
        }
    }
}
