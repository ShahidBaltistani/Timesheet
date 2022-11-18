using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class TicketStatusChangedToConversationStatusInTicketEntity : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.Tickets", "statusid", "dbo.TicketStatus");
            AddForeignKey("dbo.Tickets", "statusid", "dbo.ConversationStatus", "id");
        }

        public override void Down()
        {
            DropForeignKey("dbo.Tickets", "statusid", "dbo.ConversationStatus");
            AddForeignKey("dbo.Tickets", "statusid", "dbo.TicketStatus", "id");
        }
    }
}