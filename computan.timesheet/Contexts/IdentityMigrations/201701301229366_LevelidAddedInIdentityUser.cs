using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class LevelidAddedInIdentityUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "Levelid", c => c.Long(false));
        }

        public override void Down()
        {
            DropColumn("dbo.Users", "Levelid");
        }
    }
}