using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class AddisRestrictFlagInApplicationUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "IsRestrictEntertime", c => c.Boolean(false));
        }

        public override void Down()
        {
            DropColumn("dbo.Users", "IsRestrictEntertime");
        }
    }
}