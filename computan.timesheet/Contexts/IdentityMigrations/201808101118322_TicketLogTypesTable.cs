using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class TicketLogTypesTable : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.TicketLogTypes",
                    c => new
                    {
                        id = c.Long(false, true),
                        name = c.String(),
                        createdonutc = c.DateTime(false),
                        updatedonutc = c.DateTime(),
                        ipused = c.String(maxLength: 20),
                        userid = c.String()
                    })
                .PrimaryKey(t => t.id);
        }

        public override void Down()
        {
            DropTable("dbo.TicketLogTypes");
        }
    }
}