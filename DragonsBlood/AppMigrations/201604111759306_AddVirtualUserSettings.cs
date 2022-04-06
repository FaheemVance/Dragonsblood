namespace DragonsBlood.AppMigrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddVirtualUserSettings : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Settings_InitialChatMessagesToDisplay", c => c.Int(nullable: false));
            AddColumn("dbo.AspNetUsers", "Settings_ShowMiniChat", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "Settings_ChatNameColor", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Settings_ChatNameColor");
            DropColumn("dbo.AspNetUsers", "Settings_ShowMiniChat");
            DropColumn("dbo.AspNetUsers", "Settings_InitialChatMessagesToDisplay");
        }
    }
}
