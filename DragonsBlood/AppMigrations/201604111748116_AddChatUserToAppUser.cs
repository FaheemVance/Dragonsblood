namespace DragonsBlood.AppMigrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddChatUserToAppUser : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChatUsers",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        Banned = c.Boolean(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Connections",
                c => new
                    {
                        Id = c.String(nullable: false, maxLength: 128),
                        UserAgent = c.String(),
                        Connected = c.Boolean(nullable: false),
                        ConnectionTime = c.DateTime(nullable: false),
                        ChatUser_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ChatUsers", t => t.ChatUser_Id)
                .Index(t => t.ChatUser_Id);
            
            AddColumn("dbo.AspNetUsers", "ChatUser_Id", c => c.Int());
            CreateIndex("dbo.AspNetUsers", "ChatUser_Id");
            AddForeignKey("dbo.AspNetUsers", "ChatUser_Id", "dbo.ChatUsers", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AspNetUsers", "ChatUser_Id", "dbo.ChatUsers");
            DropForeignKey("dbo.Connections", "ChatUser_Id", "dbo.ChatUsers");
            DropIndex("dbo.Connections", new[] { "ChatUser_Id" });
            DropIndex("dbo.AspNetUsers", new[] { "ChatUser_Id" });
            DropColumn("dbo.AspNetUsers", "ChatUser_Id");
            DropTable("dbo.Connections");
            DropTable("dbo.ChatUsers");
        }
    }
}
