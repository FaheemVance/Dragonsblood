namespace DragonsBlood.AppMigrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddCurrentRoomToChatUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChatUsers", "CurrentRoom", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ChatUsers", "CurrentRoom");
        }
    }
}
