namespace DragonsBlood.AppMigrations
{
    using System.Data.Entity.Migrations;
    
    public partial class RemoveUserSettings : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "Settings_InitialChatMessagesToDisplay");
            DropColumn("dbo.AspNetUsers", "Settings_ShowMiniChat");
            DropColumn("dbo.AspNetUsers", "Settings_ChatNameColor");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Settings_ChatNameColor", c => c.String());
            AddColumn("dbo.AspNetUsers", "Settings_ShowMiniChat", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "Settings_InitialChatMessagesToDisplay", c => c.Int(nullable: false));
        }
    }
}
