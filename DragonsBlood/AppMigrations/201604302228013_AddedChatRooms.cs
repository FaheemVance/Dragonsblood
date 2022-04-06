namespace DragonsBlood.AppMigrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddedChatRooms : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChatRooms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.ChatUsers", "ChatRoom_Id", c => c.Int());
            CreateIndex("dbo.ChatUsers", "ChatRoom_Id");
            AddForeignKey("dbo.ChatUsers", "ChatRoom_Id", "dbo.ChatRooms", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChatUsers", "ChatRoom_Id", "dbo.ChatRooms");
            DropIndex("dbo.ChatUsers", new[] { "ChatRoom_Id" });
            DropColumn("dbo.ChatUsers", "ChatRoom_Id");
            DropTable("dbo.ChatRooms");
        }
    }
}
