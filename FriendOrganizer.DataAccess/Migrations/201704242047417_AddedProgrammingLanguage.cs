namespace FriendOrganizer.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedProgrammingLanguage : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ProgrammingLanguage",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(nullable: false, maxLength: 50),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Friend", "FavoriteLanguageId", c => c.Int());
            CreateIndex("dbo.Friend", "FavoriteLanguageId");
            AddForeignKey("dbo.Friend", "FavoriteLanguageId", "dbo.ProgrammingLanguage", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Friend", "FavoriteLanguageId", "dbo.ProgrammingLanguage");
            DropIndex("dbo.Friend", new[] { "FavoriteLanguageId" });
            DropColumn("dbo.Friend", "FavoriteLanguageId");
            DropTable("dbo.ProgrammingLanguage");
        }
    }
}
