namespace PracaAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class UpdatedModel : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Tags", "Name", c => c.String(nullable: false));
            AlterColumn("dbo.SongClipUrls", "ClipUrl", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SongClipUrls", "ClipUrl", c => c.String(nullable: false));
            AlterColumn("dbo.Tags", "Name", c => c.String());
        }
    }
}
