namespace DragonsBlood.AppMigrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddNewSettingsForSounds : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "Settings_PingForMessages", c => c.Boolean(nullable: false));
            AddColumn("dbo.AspNetUsers", "Settings_PulseForAlerts", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "Settings_PulseForAlerts");
            DropColumn("dbo.AspNetUsers", "Settings_PingForMessages");
        }
    }
}
