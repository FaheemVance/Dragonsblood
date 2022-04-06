namespace DragonsBlood.AppMigrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ModifyChatStore : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Connections", "ChatUser_Id", "dbo.ChatUsers");
            DropIndex("dbo.Connections", new[] { "ChatUser_Id" });
            DropPrimaryKey("dbo.Connections");
            CreateTable(
                "dbo.Sessions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserAgent = c.String(),
                        ChatUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.ChatUsers", t => t.ChatUserId, cascadeDelete: true)
                .Index(t => t.ChatUserId);
            
            AddColumn("dbo.Connections", "SessionId", c => c.Int(nullable: false));
            AddColumn("dbo.Connections", "ChatConnectionId", c => c.String());
            AlterColumn("dbo.Connections", "Id", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.Connections", "Id");
            CreateIndex("dbo.Connections", "SessionId");
            AddForeignKey("dbo.Connections", "SessionId", "dbo.Sessions", "Id", cascadeDelete: true);
            DropColumn("dbo.Connections", "UserAgent");
            DropColumn("dbo.Connections", "ChatUser_Id");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Connections", "ChatUser_Id", c => c.Int());
            AddColumn("dbo.Connections", "UserAgent", c => c.String());
            DropForeignKey("dbo.Connections", "SessionId", "dbo.Sessions");
            DropForeignKey("dbo.Sessions", "ChatUserId", "dbo.ChatUsers");
            DropIndex("dbo.Connections", new[] { "SessionId" });
            DropIndex("dbo.Sessions", new[] { "ChatUserId" });
            DropPrimaryKey("dbo.Connections");
            DropColumn("dbo.Connections", "ChatConnectionId");
            AlterColumn("dbo.Connections", "Id", c => c.String(nullable: false, maxLength: 128));
            DropColumn("dbo.Connections", "SessionId");
            DropTable("dbo.Sessions");
            AddPrimaryKey("dbo.Connections", "Id");
            CreateIndex("dbo.Connections", "ChatUser_Id");
            AddForeignKey("dbo.Connections", "ChatUser_Id", "dbo.ChatUsers", "Id");
        }
    }
}
