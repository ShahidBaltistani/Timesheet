using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class NextLinkTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.NextLinks",
                    c => new
                    {
                        id = c.Long(false, true),
                        url = c.String()
                    })
                .PrimaryKey(t => t.id);
        }

        public override void Down()
        {
            DropTable("dbo.NextLinks");
        }
    }
}