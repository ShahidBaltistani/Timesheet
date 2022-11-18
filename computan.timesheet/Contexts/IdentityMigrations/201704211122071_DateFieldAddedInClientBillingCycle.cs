using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class DateFieldAddedInClientBillingCycle : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ClientBillingCycles", "date", c => c.Int());
            AlterColumn("dbo.Clients", "maxbillablehours", c => c.Double());
            AlterColumn("dbo.ClientBillingInvoices", "hoursconsumed", c => c.Double(false));
        }

        public override void Down()
        {
            AlterColumn("dbo.ClientBillingInvoices", "hoursconsumed", c => c.Int(false));
            AlterColumn("dbo.Clients", "maxbillablehours", c => c.Int());
            DropColumn("dbo.ClientBillingCycles", "date");
        }
    }
}