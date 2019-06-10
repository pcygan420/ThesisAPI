namespace PracaAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class FavouritesFix : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Favourites", "UserId", c => c.String(maxLength: 128));
            CreateIndex("dbo.Favourites", "UserId");
            AddForeignKey("dbo.Favourites", "UserId", "dbo.User", "UserId");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Favourites", "UserId", "dbo.User");
            DropIndex("dbo.Favourites", new[] { "UserId" });
            AlterColumn("dbo.Favourites", "UserId", c => c.String());
        }
    }
}
