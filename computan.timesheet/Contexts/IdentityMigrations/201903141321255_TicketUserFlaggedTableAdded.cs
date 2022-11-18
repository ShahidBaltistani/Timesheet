using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class TicketUserFlaggedTableAdded : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.TicketUserFlaggeds",
                    c => new
                    {
                        id = c.Long(false, true),
                        ticketid = c.Long(false),
                        userid = c.String(),
                        isactive = c.Boolean(false)
                    })
                .PrimaryKey(t => t.id);
        }

        public override void Down()
        {
            DropTable("dbo.TicketUserFlaggeds");
        }
    }
}