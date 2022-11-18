using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class TicketitemCcrecipientsAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TicketItems", "ccrecipients", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.TicketItems", "ccrecipients");
        }
    }
}