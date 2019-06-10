namespace PracaAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class LastFix : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.AddedSongs", name: "TextUserId", newName: "UserId");
            RenameIndex(table: "dbo.AddedSongs", name: "IX_TextUserId", newName: "IX_UserId");
            AlterColumn("dbo.Comments", "Text", c => c.String(nullable: false, maxLength: 500));
            AlterColumn("dbo.SongClipUrls", "ClipUrl", c => c.String(nullable: false));
        }
        
        public override void Down()
        {
            AlterColumn("dbo.SongClipUrls", "ClipUrl", c => c.String());
            AlterColumn("dbo.Comments", "Text", c => c.String(maxLength: 500));
            RenameIndex(table: "dbo.AddedSongs", name: "IX_UserId", newName: "IX_TextUserId");
            RenameColumn(table: "dbo.AddedSongs", name: "UserId", newName: "TextUserId");
        }
    }
}
