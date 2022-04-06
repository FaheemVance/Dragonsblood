namespace DragonsBlood.AppMigrations
{
    using System.Data.Entity.Migrations;
    
    public partial class ChatDbsAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChatMessages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        User = c.String(),
                        Message = c.String(),
                        TimeStamp = c.DateTime(nullable: false),
                        Removed = c.Boolean(nullable: false),
                        Edited = c.Boolean(nullable: false),
                        EditedBy = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ChatMessages");
        }
    }
}
