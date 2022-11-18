using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class TicketEstimateTimeLog : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                    "dbo.TicketEstimateTimeLogs",
                    c => new
                    {
                        id = c.Long(false, true),
                        ticketid = c.Long(false),
                        ticketitemcount = c.Int(false),
                        timeestimateinminutes = c.Int(false),
                        ticketusers = c.String(),
                        createdonutc = c.DateTime(false),
                        updatedonutc = c.DateTime(),
                        ipused = c.String(maxLength: 20),
                        userid = c.String()
                    })
                .PrimaryKey(t => t.id);

            AlterColumn("dbo.TimeEntryLogs", "userid", c => c.String(maxLength: 128));
            CreateIndex("dbo.TimeEntryLogs", "userid");
            AddForeignKey("dbo.TimeEntryLogs", "userid", "dbo.Users", "UsersId");
        }

        public override void Down()
        {
            DropForeignKey("dbo.TimeEntryLogs", "userid", "dbo.Users");
            DropIndex("dbo.TimeEntryLogs", new[] { "userid" });
            AlterColumn("dbo.TimeEntryLogs", "userid", c => c.String());
            DropTable("dbo.TicketEstimateTimeLogs");
        }
    }
}