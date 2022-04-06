namespace DragonsBlood.AppMigrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ModifyConnectionStore : DbMigration
    {
        public override void Up()
        {

            DropTable("dbo.Connections");
            CreateTable(
               "dbo.Connections",
               c => new
               {
                   Id = c.Int(nullable: false, identity: true),
                   Connected = c.Boolean(nullable: false),
                   ConnectionTime = c.DateTime(nullable: false),
                   ChatConnectionId = c.String(nullable: false),
                   SessionId = c.Int(),
               })
               .PrimaryKey(t => t.Id)
               .ForeignKey("dbo.Sessions", t => t.SessionId)
               .Index(t => t.SessionId);
        }

        public override void Down()
        {
            DropTable("dbo.Connections");
            CreateTable(
               "dbo.Connections",
               c => new
               {
                   Id = c.Guid(nullable: false, identity: true),
                   UserAgent = c.String(),
                   Connected = c.Boolean(nullable: false),
                   ConnectionTime = c.DateTime(nullable: false),
                   SessionId = c.Int(),
               })
               .PrimaryKey(t => t.Id)
               .ForeignKey("dbo.Sessions", t => t.SessionId)
               .Index(t => t.SessionId);
        }
    }
}
