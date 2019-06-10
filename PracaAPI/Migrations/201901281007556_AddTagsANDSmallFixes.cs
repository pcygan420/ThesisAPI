namespace PracaAPI.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddTagsANDSmallFixes : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Tags",
                c => new
                    {
                        TagId = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Entity_EntityId = c.Int(),
                    })
                .PrimaryKey(t => t.TagId)
                .ForeignKey("dbo.Entities", t => t.Entity_EntityId)
                .Index(t => t.Entity_EntityId);
            
            AddColumn("dbo.Songs", "Curiosities", c => c.String());
            AddColumn("dbo.SongMetrics", "Curosities", c => c.String());
            AddColumn("dbo.SongMetrics", "Tags", c => c.String());
            DropColumn("dbo.Songs", "Genres");
            DropColumn("dbo.SongMetrics", "Genres");
        }
        
        public override void Down()
        {
            AddColumn("dbo.SongMetrics", "Genres", c => c.String());
            AddColumn("dbo.Songs", "Genres", c => c.String());
            DropForeignKey("dbo.Tags", "Entity_EntityId", "dbo.Entities");
            DropIndex("dbo.Tags", new[] { "Entity_EntityId" });
            DropColumn("dbo.SongMetrics", "Tags");
            DropColumn("dbo.SongMetrics", "Curosities");
            DropColumn("dbo.Songs", "Curiosities");
            DropTable("dbo.Tags");
        }
    }
}
