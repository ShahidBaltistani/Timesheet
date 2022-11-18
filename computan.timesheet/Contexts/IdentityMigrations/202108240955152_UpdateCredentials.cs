using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class UpdateCredentials : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Credentials", "isactive", c => c.Boolean(false, true));
            AlterColumn("dbo.TicketEstimateTimeLogs", "userid", c => c.String(maxLength: 128));
            CreateIndex("dbo.TicketEstimateTimeLogs", "userid");
            AddForeignKey("dbo.TicketEstimateTimeLogs", "userid", "dbo.Users", "UsersId");
        }

        public override void Down()
        {
            DropForeignKey("dbo.TicketEstimateTimeLogs", "userid", "dbo.Users");
            DropIndex("dbo.TicketEstimateTimeLogs", new[] { "userid" });
            AlterColumn("dbo.TicketEstimateTimeLogs", "userid", c => c.String());
            DropColumn("dbo.Credentials", "isactive");
        }
    }
}