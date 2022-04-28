namespace FriendOrganizer.DataAccess.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedMeeting : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Meeting",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Title = c.String(nullable: false, maxLength: 50),
                        DateFrom = c.DateTime(nullable: false),
                        DateTo = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.MeetingFriend",
                c => new
                    {
                        Meeting_Id = c.Int(nullable: false),
                        Friend_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.Meeting_Id, t.Friend_Id })
                .ForeignKey("dbo.Meeting", t => t.Meeting_Id, cascadeDelete: true)
                .ForeignKey("dbo.Friend", t => t.Friend_Id, cascadeDelete: true)
                .Index(t => t.Meeting_Id)
                .Index(t => t.Friend_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MeetingFriend", "Friend_Id", "dbo.Friend");
            DropForeignKey("dbo.MeetingFriend", "Meeting_Id", "dbo.Meeting");
            DropIndex("dbo.MeetingFriend", new[] { "Friend_Id" });
            DropIndex("dbo.MeetingFriend", new[] { "Meeting_Id" });
            DropTable("dbo.MeetingFriend");
            DropTable("dbo.Meeting");
        }
    }
}
