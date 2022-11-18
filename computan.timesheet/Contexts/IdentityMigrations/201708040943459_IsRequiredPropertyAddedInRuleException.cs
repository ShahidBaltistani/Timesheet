using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class IsRequiredPropertyAddedInRuleException : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.RuleExceptions", "isrequired", c => c.Boolean(false));
        }

        public override void Down()
        {
            DropColumn("dbo.RuleExceptions", "isrequired");
        }
    }
}