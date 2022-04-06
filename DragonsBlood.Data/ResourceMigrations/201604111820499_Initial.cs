namespace DragonsBlood.Data.ResourceMigrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Initial : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Alliances",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        State = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Announcements",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Subject = c.String(),
                        Body = c.String(),
                        Stamp = c.DateTime(nullable: false),
                        Type = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Documents",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        DisplayText = c.String(),
                        Location = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Enemies",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        State = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.Enemies");
            DropTable("dbo.Documents");
            DropTable("dbo.Announcements");
            DropTable("dbo.Alliances");
        }
    }
}
