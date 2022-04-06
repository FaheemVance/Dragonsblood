namespace DragonsBlood.AppMigrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddMapFunctionality : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.MapItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        UserName = c.String(),
                        X = c.Int(nullable: false),
                        Y = c.Int(nullable: false),
                        Level = c.Int(nullable: false),
                        Alliance = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.MapItems");
        }
    }
}
