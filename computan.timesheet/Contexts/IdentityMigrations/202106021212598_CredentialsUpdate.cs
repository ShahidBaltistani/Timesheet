using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class CredentialsUpdate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Credentials", "linkedCredential", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.Credentials", "linkedCredential");
        }
    }
}