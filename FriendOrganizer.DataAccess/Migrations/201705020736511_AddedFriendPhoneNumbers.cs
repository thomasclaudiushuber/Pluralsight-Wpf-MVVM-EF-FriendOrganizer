namespace FriendOrganizer.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedFriendPhoneNumbers : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FriendPhoneNumber",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Number = c.String(nullable: false),
                        FriendId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Friend", t => t.FriendId, cascadeDelete: true)
                .Index(t => t.FriendId);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.FriendPhoneNumber", "FriendId", "dbo.Friend");
            DropIndex("dbo.FriendPhoneNumber", new[] { "FriendId" });
            DropTable("dbo.FriendPhoneNumber");
        }
    }
}
