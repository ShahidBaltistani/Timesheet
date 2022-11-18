using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class displayorderaddedintickteitemlog : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TicketItemLogs", "displayorder", c => c.Long());
        }

        public override void Down()
        {
            DropColumn("dbo.TicketItemLogs", "displayorder");
        }
    }
}