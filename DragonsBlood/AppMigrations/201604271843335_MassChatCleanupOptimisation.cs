namespace DragonsBlood.AppMigrations
{
    using System.Data.Entity.Migrations;
    
    public partial class MassChatCleanupOptimisation : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Sessions", "ChatUserId", "dbo.ChatUsers");
            DropForeignKey("dbo.Connections", "SessionId", "dbo.Sessions");
            DropIndex("dbo.Sessions", new[] { "ChatUserId" });
            DropIndex("dbo.Connections", new[] { "SessionId" });
            AddColumn("dbo.Connections", "ConnectionId", c => c.String());
            AddColumn("dbo.Connections", "UserAgent", c => c.String());
            AddColumn("dbo.Connections", "ChatUser_Id", c => c.Int());
            CreateIndex("dbo.Connections", "ChatUser_Id");
            AddForeignKey("dbo.Connections", "ChatUser_Id", "dbo.ChatUsers", "Id");
            DropColumn("dbo.Connections", "ConnectionTime");
            DropColumn("dbo.Connections", "ChatConnectionId");
            DropColumn("dbo.Connections", "SessionId");
            DropTable("dbo.Sessions");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.Sessions",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserAgent = c.String(),
                        CreationStamp = c.DateTime(nullable: false),
                        ChatUserId = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Connections", "SessionId", c => c.Int(nullable: false));
            AddColumn("dbo.Connections", "ChatConnectionId", c => c.String());
            AddColumn("dbo.Connections", "ConnectionTime", c => c.DateTime(nullable: false));
            DropForeignKey("dbo.Connections", "ChatUser_Id", "dbo.ChatUsers");
            DropIndex("dbo.Connections", new[] { "ChatUser_Id" });
            DropColumn("dbo.Connections", "ChatUser_Id");
            DropColumn("dbo.Connections", "UserAgent");
            DropColumn("dbo.Connections", "ConnectionId");
            CreateIndex("dbo.Connections", "SessionId");
            CreateIndex("dbo.Sessions", "ChatUserId");
            AddForeignKey("dbo.Connections", "SessionId", "dbo.Sessions", "Id", cascadeDelete: true);
            AddForeignKey("dbo.Sessions", "ChatUserId", "dbo.ChatUsers", "Id", cascadeDelete: true);
        }
    }
}
