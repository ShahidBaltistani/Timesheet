using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class CustomerSourceTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.CustomerSources",
                    c => new
                    {
                        id = c.Long(false, true),
                        name = c.String(),
                        isactive = c.Boolean(false),
                        createdonutc = c.DateTime(false),
                        updatedonutc = c.DateTime(),
                        ipused = c.String(maxLength: 20),
                        userid = c.String()
                    })
                .PrimaryKey(t => t.id);
        }

        public override void Down()
        {
            DropTable("dbo.CustomerSources");
        }
    }
}