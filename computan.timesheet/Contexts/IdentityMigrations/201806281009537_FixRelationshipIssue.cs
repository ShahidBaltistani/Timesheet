using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class FixRelationshipIssue : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.TicketTeamLogs", "ticketid", "dbo.Tickets");
            DropIndex("dbo.TicketTeamLogs", new[] { "ticketid" });
            CreateIndex("dbo.TicketTeamLogs", "ticketid");
            AddForeignKey("dbo.TicketTeamLogs", "ticketid", "dbo.Tickets", "id");
        }

        public override void Down()
        {
            DropForeignKey("dbo.TicketTeamLogs", "ticketid", "dbo.Tickets");
            DropIndex("dbo.TicketTeamLogs", new[] { "ticketid" });
        }
    }
}