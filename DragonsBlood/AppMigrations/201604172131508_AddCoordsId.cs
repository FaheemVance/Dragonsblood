namespace DragonsBlood.AppMigrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddCoordsId : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Coordinates",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        X = c.Int(nullable: false),
                        Y = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            AddColumn("dbo.Alerts", "Coordinates_Id", c => c.Int(nullable: false));
            AlterColumn("dbo.Alerts", "Attacker", c => c.String(nullable: false));
            CreateIndex("dbo.Alerts", "Coordinates_Id");
            AddForeignKey("dbo.Alerts", "Coordinates_Id", "dbo.Coordinates", "Id", cascadeDelete: true);
            DropColumn("dbo.Alerts", "Coordinates_X");
            DropColumn("dbo.Alerts", "Coordinates_Y");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Alerts", "Coordinates_Y", c => c.Int(nullable: false));
            AddColumn("dbo.Alerts", "Coordinates_X", c => c.Int(nullable: false));
            DropForeignKey("dbo.Alerts", "Coordinates_Id", "dbo.Coordinates");
            DropIndex("dbo.Alerts", new[] { "Coordinates_Id" });
            AlterColumn("dbo.Alerts", "Attacker", c => c.String());
            DropColumn("dbo.Alerts", "Coordinates_Id");
            DropTable("dbo.Coordinates");
        }
    }
}
