namespace DragonsBlood.AppMigrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedMotdToRooms : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChatRooms", "Motd", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.ChatRooms", "Motd");
        }
    }
}
