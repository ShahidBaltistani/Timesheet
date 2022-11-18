using System.Data.Entity.Migrations;

namespace computan.timesheet.Contexts.IdentityMigrations
{
    public partial class TitlePropertyAddedInCredentialsEntity : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Credentials", "title", c => c.String());
        }

        public override void Down()
        {
            DropColumn("dbo.Credentials", "title");
        }
    }
}