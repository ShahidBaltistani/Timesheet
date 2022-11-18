using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class YearMaxBillablehoursHoursConsumedPropertiesAddedInBillingNotificationEntity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BillingNotifications", "billingyear", c => c.String());
            AddColumn("dbo.BillingNotifications", "maxbillablehours", c => c.Double());
            AddColumn("dbo.BillingNotifications", "hoursconsumed", c => c.Double());
        }

        public override void Down()
        {
            DropColumn("dbo.BillingNotifications", "hoursconsumed");
            DropColumn("dbo.BillingNotifications", "maxbillablehours");
            DropColumn("dbo.BillingNotifications", "billingyear");
        }
    }
}