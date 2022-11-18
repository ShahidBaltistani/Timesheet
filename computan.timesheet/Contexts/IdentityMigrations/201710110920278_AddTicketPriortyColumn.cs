using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class AddTicketPriortyColumn : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Tickets", "ticketpriortyid", c => c.Int(false));
        }

        public override void Down()
        {
            DropColumn("dbo.Tickets", "ticketpriortyid");
        }
    }
}