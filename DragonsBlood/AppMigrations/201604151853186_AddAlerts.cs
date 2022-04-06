namespace DragonsBlood.AppMigrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddAlerts : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Alerts",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Attacker = c.String(),
                        Kingdom = c.Int(nullable: false),
                        ShortKingdom = c.Int(nullable: false),
                        Coordinates_X = c.Int(nullable: false),
                        Coordinates_Y = c.Int(nullable: false),
                        TimeStamp = c.DateTime(nullable: false),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .Index(t => t.ApplicationUser_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Alerts", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.Alerts", new[] { "ApplicationUser_Id" });
            DropTable("dbo.Alerts");
        }
    }
}
