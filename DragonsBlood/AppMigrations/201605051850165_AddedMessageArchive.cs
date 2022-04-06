namespace DragonsBlood.AppMigrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddedMessageArchive : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ChatMessageArchives",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Message = c.String(),
                        User = c.String(),
                        RoomName = c.String(),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.ChatMessageArchives");
        }
    }
}
