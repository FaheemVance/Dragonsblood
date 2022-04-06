namespace DragonsBlood.AppMigrations
{
    using System.Data.Entity.Migrations;
    
    public partial class UpdateConnectionStore : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Connections", "ConnectionTime", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Connections", "ConnectionTime");
        }
    }
}
