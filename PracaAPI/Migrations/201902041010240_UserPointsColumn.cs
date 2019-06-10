namespace PracaAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UserPointsColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.User", "Points", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.User", "Points");
        }
    }
}
