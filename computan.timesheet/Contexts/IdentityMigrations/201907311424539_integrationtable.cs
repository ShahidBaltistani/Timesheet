using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class integrationtable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.Integrations",
                    c => new
                    {
                        id = c.Long(false, true),
                        name = c.String(),
                        appsettings = c.String(),
                        isenabled = c.Boolean(false),
                        createdonutc = c.DateTime(false),
                        updatedonutc = c.DateTime(),
                        ipused = c.String(maxLength: 20),
                        userid = c.String()
                    })
                .PrimaryKey(t => t.id);
        }

        public override void Down()
        {
            CreateIndex("dbo.TicketComments", "parentid");
        }
    }
}