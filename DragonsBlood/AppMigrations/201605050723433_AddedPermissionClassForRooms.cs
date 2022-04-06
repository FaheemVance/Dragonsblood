namespace DragonsBlood.AppMigrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddedPermissionClassForRooms : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChatRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Description = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ChatRoomPermissions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Role_Id = c.Int(),
                        Room_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ChatRoles", t => t.Role_Id)
                .ForeignKey("dbo.ChatRooms", t => t.Room_Id)
                .Index(t => t.Role_Id)
                .Index(t => t.Room_Id);
            
            CreateTable(
                "dbo.ChatUserRoles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Role_Id = c.Int(),
                        User_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ChatRoles", t => t.Role_Id)
                .ForeignKey("dbo.ChatUsers", t => t.User_Id)
                .Index(t => t.Role_Id)
                .Index(t => t.User_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ChatUserRoles", "User_Id", "dbo.ChatUsers");
            DropForeignKey("dbo.ChatUserRoles", "Role_Id", "dbo.ChatRoles");
            DropForeignKey("dbo.ChatRoomPermissions", "Room_Id", "dbo.ChatRooms");
            DropForeignKey("dbo.ChatRoomPermissions", "Role_Id", "dbo.ChatRoles");
            DropIndex("dbo.ChatUserRoles", new[] { "User_Id" });
            DropIndex("dbo.ChatUserRoles", new[] { "Role_Id" });
            DropIndex("dbo.ChatRoomPermissions", new[] { "Room_Id" });
            DropIndex("dbo.ChatRoomPermissions", new[] { "Role_Id" });
            DropTable("dbo.ChatUserRoles");
            DropTable("dbo.ChatRoomPermissions");
            DropTable("dbo.ChatRoles");
        }
    }
}
