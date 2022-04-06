namespace DragonsBlood.AppMigrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddedLinkBetweenRoomsAndUsers : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.ChatUsers", "ChatRoom_Id", "dbo.ChatRooms");
            DropIndex("dbo.ChatUsers", new[] { "ChatRoom_Id" });
            CreateTable(
                "dbo.ChatRoomUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Room_Id = c.Int(),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ChatRooms", t => t.Room_Id)
                .ForeignKey("dbo.ChatUsers", t => t.User_Id)
                .Index(t => t.Room_Id)
                .Index(t => t.User_Id);
            
            AddColumn("dbo.ChatMessages", "ChatRoom_Id", c => c.Int());
            CreateIndex("dbo.ChatMessages", "ChatRoom_Id");
            AddForeignKey("dbo.ChatMessages", "ChatRoom_Id", "dbo.ChatRooms", "Id");
            DropColumn("dbo.ChatUsers", "ChatRoom_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ChatUsers", "ChatRoom_Id", c => c.Int());
            DropForeignKey("dbo.ChatRoomUsers", "User_Id", "dbo.ChatUsers");
            DropForeignKey("dbo.ChatRoomUsers", "Room_Id", "dbo.ChatRooms");
            DropForeignKey("dbo.ChatMessages", "ChatRoom_Id", "dbo.ChatRooms");
            DropIndex("dbo.ChatRoomUsers", new[] { "User_Id" });
            DropIndex("dbo.ChatRoomUsers", new[] { "Room_Id" });
            DropIndex("dbo.ChatMessages", new[] { "ChatRoom_Id" });
            DropColumn("dbo.ChatMessages", "ChatRoom_Id");
            DropTable("dbo.ChatRoomUsers");
            CreateIndex("dbo.ChatUsers", "ChatRoom_Id");
            AddForeignKey("dbo.ChatUsers", "ChatRoom_Id", "dbo.ChatRooms", "Id");
        }
    }
}
