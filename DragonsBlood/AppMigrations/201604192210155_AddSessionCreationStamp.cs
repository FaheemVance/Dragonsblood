namespace DragonsBlood.AppMigrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddSessionCreationStamp : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Sessions", "CreationStamp", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Sessions", "CreationStamp");
        }
    }
}
