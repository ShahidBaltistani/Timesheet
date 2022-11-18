using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class BrowserInfoTableAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.UserBrowserinfoes",
                    c => new
                    {
                        id = c.Long(false, true),
                        browser = c.String(),
                        userId = c.String(),
                        token = c.String(),
                        isActive = c.Boolean(false)
                    })
                .PrimaryKey(t => t.id);
        }

        public override void Down()
        {
            DropTable("dbo.UserBrowserinfoes");
        }
    }
}