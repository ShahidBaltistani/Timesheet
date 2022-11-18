using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class TeamCodeFieldAdded : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Teams", "code", c => c.String(maxLength: 3));
        }

        public override void Down()
        {
            DropColumn("dbo.Teams", "code");
        }
    }
}