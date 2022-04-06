namespace DragonsBlood.AppMigrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddBoolToCloseAlert : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Alerts", "Retaliated", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Alerts", "Retaliated");
        }
    }
}
