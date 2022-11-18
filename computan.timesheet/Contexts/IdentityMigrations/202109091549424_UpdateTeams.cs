using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class UpdateTeams : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Teams", "displayorder", c => c.Int());
        }

        public override void Down()
        {
            DropColumn("dbo.Teams", "displayorder");
        }
    }
}