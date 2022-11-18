using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class IsSentPropertyAddedInBillingNotification : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.BillingNotifications", "issent", c => c.Boolean(false));
            AddColumn("dbo.BillingNotifications", "issentagain", c => c.Boolean(false));
        }

        public override void Down()
        {
            DropColumn("dbo.BillingNotifications", "issentagain");
            DropColumn("dbo.BillingNotifications", "issent");
        }
    }
}