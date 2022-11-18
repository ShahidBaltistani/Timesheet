using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class AddIsActiveFlagtoMemberteam : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TeamMembers", "IsActive", c => c.Boolean(false));
        }

        public override void Down()
        {
            DropColumn("dbo.TeamMembers", "IsActive");
        }
    }
}