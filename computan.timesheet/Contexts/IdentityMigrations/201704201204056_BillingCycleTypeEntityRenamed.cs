using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class BillingCycleTypeEntityRenamed : DbMigration
    {
        public override void Up()
        {
            RenameTable("dbo.BillingCyleTypes", "BillingCycleTypes");
        }

        public override void Down()
        {
            RenameTable("dbo.BillingCycleTypes", "BillingCyleTypes");
        }
    }
}