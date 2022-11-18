using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class credentialsencrypt : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Credentials", "passwordhash", c => c.String(false));
            AddColumn("dbo.Credentials", "passwordsalt", c => c.String(false));
        }

        public override void Down()
        {
            DropColumn("dbo.Credentials", "passwordsalt");
            DropColumn("dbo.Credentials", "passwordhash");
        }
    }
}