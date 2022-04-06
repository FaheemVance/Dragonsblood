namespace DragonsBlood.AppMigrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddedAbilityToMarkRolesAsDefault : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ChatRoles", "IsDefault", c => c.Boolean(nullable: false));
            AddColumn("dbo.ChatRooms", "IsDefault", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ChatRooms", "IsDefault");
            DropColumn("dbo.ChatRoles", "IsDefault");
        }
    }
}
