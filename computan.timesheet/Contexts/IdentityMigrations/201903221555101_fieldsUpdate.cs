using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class fieldsUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "IsNotifyManagerOnTaskAssignment", c => c.Boolean(false));
            AddColumn("dbo.TeamMembers", "IsManager", c => c.Boolean(false));
        }

        public override void Down()
        {
            DropColumn("dbo.TeamMembers", "IsManager");
            DropColumn("dbo.Users", "IsNotifyManagerOnTaskAssignment");
        }
    }
}