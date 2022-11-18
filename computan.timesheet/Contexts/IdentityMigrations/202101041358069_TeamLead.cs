using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class TeamLead : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.TeamMembers", "IsTeamLead", c => c.Boolean(false));
            AddColumn("dbo.TeamMembers", "Reported", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.TeamMembers", "Reported");
            DropColumn("dbo.TeamMembers", "IsTeamLead");
        }
    }
}