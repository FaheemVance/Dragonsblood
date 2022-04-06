namespace DragonsBlood.Data.ResourceMigrations
{
    using System.Data.Entity.Migrations;
    
    public partial class AddedFeedbackFunctionality : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.FeedbackItems",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Creator = c.String(),
                        Comment = c.String(),
                        TimeStamp = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.FeedbackItems");
        }
    }
}
