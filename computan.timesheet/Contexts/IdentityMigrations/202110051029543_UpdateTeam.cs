using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class UpdateTeam : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Teams", "Manager", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.Teams", "Manager");
        }
    }
}