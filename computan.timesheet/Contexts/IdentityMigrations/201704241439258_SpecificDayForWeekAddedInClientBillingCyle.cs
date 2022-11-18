using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class SpecificDayForWeekAddedInClientBillingCyle : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClientBillingCycles", "day", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.ClientBillingCycles", "day");
        }
    }
}