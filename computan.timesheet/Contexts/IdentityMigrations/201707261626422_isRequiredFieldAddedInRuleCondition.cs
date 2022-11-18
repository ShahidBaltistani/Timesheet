using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class isRequiredFieldAddedInRuleCondition : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RuleConditions", "isrequired", c => c.Boolean(false));
        }

        public override void Down()
        {
            DropColumn("dbo.RuleConditions", "isrequired");
        }
    }
}