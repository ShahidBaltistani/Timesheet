using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class IsActivePropertyAddedInIdentityUser : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "isactive", c => c.Boolean(false));
        }

        public override void Down()
        {
            DropColumn("dbo.Users", "isactive");
        }
    }
}