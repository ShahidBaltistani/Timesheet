using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class AssignedbyuserForienkeyadded : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RuleConditions", "ruleconditionvalue", c => c.String(false));
            AlterColumn("dbo.RuleExceptions", "ruleexceptionvalue", c => c.String(false));
            CreateIndex("dbo.TicketItemLogs", "assignedbyusersid");
            AddForeignKey("dbo.TicketItemLogs", "assignedbyusersid", "dbo.Users", "UsersId");
        }

        public override void Down()
        {
            DropForeignKey("dbo.TicketItemLogs", "assignedbyusersid", "dbo.Users");
            DropIndex("dbo.TicketItemLogs", new[] { "assignedbyusersid" });
            AlterColumn("dbo.RuleExceptions", "ruleexceptionvalue", c => c.String());
            AlterColumn("dbo.RuleConditions", "ruleconditionvalue", c => c.String());
        }
    }
}