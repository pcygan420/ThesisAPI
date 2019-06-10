namespace PracaAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Actions",
                c => new
                    {
                        ActionId = c.Int(nullable: false, identity: true),
                        SongId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        Date = c.DateTime(nullable: false),
                        Status = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ActionId)
                .ForeignKey("dbo.Songs", t => t.SongId)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.SongId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Entities",
                c => new
                    {
                        EntityId = c.Int(nullable: false, identity: true),
                    })
                .PrimaryKey(t => t.EntityId);
            
            CreateTable(
                "dbo.Comments",
                c => new
                    {
                        CommentId = c.Int(nullable: false, identity: true),
                        EntityId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        AddDate = c.DateTime(nullable: false),
                        Text = c.String(maxLength: 500),
                    })
                .PrimaryKey(t => t.CommentId)
                .ForeignKey("dbo.Entities", t => t.EntityId, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.EntityId)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Favourites",
                c => new
                    {
                        FavouriteId = c.Int(nullable: false, identity: true),
                        EntityId = c.Int(nullable: false),
                        UserId = c.String(),
                    })
                .PrimaryKey(t => t.FavouriteId)
                .ForeignKey("dbo.Entities", t => t.EntityId, cascadeDelete: true)
                .Index(t => t.EntityId);
            
            CreateTable(
                "dbo.Rates",
                c => new
                    {
                        RateId = c.Int(nullable: false, identity: true),
                        EntityId = c.Int(nullable: false),
                        UserId = c.String(maxLength: 128),
                        Date = c.DateTime(nullable: false),
                        Rating = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.RateId)
                .ForeignKey("dbo.Entities", t => t.EntityId, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.UserId)
                .Index(t => t.EntityId)
                .Index(t => t.UserId);

            
            CreateTable(
                "dbo.Replies",
                c => new
                    {
                        ReplyId = c.Int(nullable: false, identity: true),
                        UserId = c.String(maxLength: 128),
                        AddDate = c.DateTime(nullable: false),
                        Text = c.String(maxLength: 500),
                        Comment_CommentId = c.Int(),
                    })
                .PrimaryKey(t => t.ReplyId)
                .ForeignKey("dbo.User", t => t.UserId)
                .ForeignKey("dbo.Comments", t => t.Comment_CommentId)
                .Index(t => t.UserId)
                .Index(t => t.Comment_CommentId);
            
            CreateTable(
                "dbo.Expectants",
                c => new
                    {
                        ExpectantId = c.Int(nullable: false, identity: true),
                        SongId = c.Int(nullable: false),
                        UserId = c.String(),
                    })
                .PrimaryKey(t => t.ExpectantId)
                .ForeignKey("dbo.Songs", t => t.SongId)
                .Index(t => t.SongId);
            
            CreateTable(
                "dbo.Songs",
                c => new
                    {
                        EntityId = c.Int(nullable: false),
                        Title = c.String(nullable: false, maxLength: 128),
                        Text = c.String(nullable: false),
                        Translation = c.String(),
                        ClipUrl = c.String(),
                        Album = c.String(),
                        Performer = c.String(),
                        PublicationDate = c.String(),
                        Duration = c.String(),
                        Genres = c.String(),
                    })
                .PrimaryKey(t => t.EntityId)
                .ForeignKey("dbo.Entities", t => t.EntityId)
                .Index(t => t.EntityId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Songs", "EntityId", "dbo.Entities");
            DropForeignKey("dbo.SongTranslations", "UserId", "dbo.User");
            DropForeignKey("dbo.SongTranslations", "EntityId", "dbo.Entities");
            DropForeignKey("dbo.SongTexts", "UserId", "dbo.User");
            DropForeignKey("dbo.SongTexts", "EntityId", "dbo.Entities");
            DropForeignKey("dbo.SongMetrics", "UserId", "dbo.User");
            DropForeignKey("dbo.SongMetrics", "EntityId", "dbo.Entities");
            DropForeignKey("dbo.SongClipUrls", "UserId", "dbo.User");
            DropForeignKey("dbo.SongClipUrls", "EntityId", "dbo.Entities");
            DropForeignKey("dbo.UserRole", "RoleId", "dbo.Role");
            DropForeignKey("dbo.AddedSongs", "TextUserId", "dbo.User");
            DropForeignKey("dbo.Actions", "UserId", "dbo.User");
            DropForeignKey("dbo.Expectants", "SongId", "dbo.Songs");
            DropForeignKey("dbo.Comments", "UserId", "dbo.User");
            DropForeignKey("dbo.Replies", "Comment_CommentId", "dbo.Comments");
            DropForeignKey("dbo.Replies", "UserId", "dbo.User");
            DropForeignKey("dbo.Rates", "UserId", "dbo.User");
            DropForeignKey("dbo.UserRole", "UserId", "dbo.User");
            DropForeignKey("dbo.UserLogin", "UserId", "dbo.User");
            DropForeignKey("dbo.UserClaim", "UserId", "dbo.User");
            DropForeignKey("dbo.Rates", "EntityId", "dbo.Entities");
            DropForeignKey("dbo.Favourites", "EntityId", "dbo.Entities");
            DropForeignKey("dbo.Comments", "EntityId", "dbo.Entities");
            DropForeignKey("dbo.Actions", "SongId", "dbo.Songs");
            DropIndex("dbo.Songs", new[] { "EntityId" });
            DropIndex("dbo.SongTranslations", new[] { "UserId" });
            DropIndex("dbo.SongTranslations", new[] { "EntityId" });
            DropIndex("dbo.SongTexts", new[] { "UserId" });
            DropIndex("dbo.SongTexts", new[] { "EntityId" });
            DropIndex("dbo.SongMetrics", new[] { "UserId" });
            DropIndex("dbo.SongMetrics", new[] { "EntityId" });
            DropIndex("dbo.SongClipUrls", new[] { "UserId" });
            DropIndex("dbo.SongClipUrls", new[] { "EntityId" });
            DropIndex("dbo.Role", "RoleNameIndex");
            DropIndex("dbo.AddedSongs", new[] { "TextUserId" });
            DropIndex("dbo.Expectants", new[] { "SongId" });
            DropIndex("dbo.Replies", new[] { "Comment_CommentId" });
            DropIndex("dbo.Replies", new[] { "UserId" });
            DropIndex("dbo.UserRole", new[] { "RoleId" });
            DropIndex("dbo.UserRole", new[] { "UserId" });
            DropIndex("dbo.UserLogin", new[] { "UserId" });
            DropIndex("dbo.UserClaim", new[] { "UserId" });
            DropIndex("dbo.User", "UserNameIndex");
            DropIndex("dbo.Rates", new[] { "UserId" });
            DropIndex("dbo.Rates", new[] { "EntityId" });
            DropIndex("dbo.Favourites", new[] { "EntityId" });
            DropIndex("dbo.Comments", new[] { "UserId" });
            DropIndex("dbo.Comments", new[] { "EntityId" });
            DropIndex("dbo.Actions", new[] { "UserId" });
            DropIndex("dbo.Actions", new[] { "SongId" });
            DropTable("dbo.Songs");
            DropTable("dbo.SongTranslations");
            DropTable("dbo.SongTexts");
            DropTable("dbo.SongMetrics");
            DropTable("dbo.SongClipUrls");
            DropTable("dbo.Role");
            DropTable("dbo.AddedSongs");
            DropTable("dbo.Expectants");
            DropTable("dbo.Replies");
            DropTable("dbo.UserRole");
            DropTable("dbo.UserLogin");
            DropTable("dbo.UserClaim");
            DropTable("dbo.User");
            DropTable("dbo.Rates");
            DropTable("dbo.Favourites");
            DropTable("dbo.Comments");
            DropTable("dbo.Entities");
            DropTable("dbo.Actions");
        }
    }
}
