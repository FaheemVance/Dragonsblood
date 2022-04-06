namespace DragonsBlood.AppMigrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddedCompletedBy : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Alerts", "CompletedBy", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Alerts", "CompletedBy");
        }
    }
}
