using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class StatusidAddedInRuleAction : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.RuleActions", "statusid", c => c.Int());
        }

        public override void Down()
        {
            AlterColumn("dbo.RuleActions", "statusid", c => c.Long());
        }
    }
}