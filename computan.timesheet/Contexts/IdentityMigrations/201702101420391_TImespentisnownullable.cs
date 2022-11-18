using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class TImespentisnownullable : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.TicketTimeLogs", "timespentinminutes", c => c.Int());
        }

        public override void Down()
        {
            AlterColumn("dbo.TicketTimeLogs", "timespentinminutes", c => c.Int(false));
        }
    }
}