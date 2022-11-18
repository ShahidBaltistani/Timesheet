using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class AddedTitleFieldInCredentials : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Credentials", "title", c => c.String(false));
        }

        public override void Down()
        {
            DropColumn("dbo.Credentials", "title");
        }
    }
}